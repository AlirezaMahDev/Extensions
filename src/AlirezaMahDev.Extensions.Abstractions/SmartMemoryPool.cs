using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.ConstrainedExecution;

namespace AlirezaMahDev.Extensions.Abstractions;

public sealed class SmartMemoryPool
{
    public const int MinSize = ushort.MaxValue;
    public static SmartMemoryPool<T> GetShared<T>() => SmartMemoryPool<T>.Shared;
}

public sealed class SmartMemoryPool<T> : MemoryPool<T>
{
    public override int MaxBufferSize => int.MaxValue;
    public static new SmartMemoryPool<T> Shared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = new();

    private readonly SmartMemoryPoolAllocation _allocation = new(SmartMemoryPool.MinSize);

    public override IMemoryOwner<T> Rent(int size)
    {
        return _allocation.Rent(size);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _allocation.Dispose();
        }
    }


    internal sealed class SmartMemoryPoolAllocation : CriticalFinalizerObject, IDisposable
    {
        private bool _disposed;
        private Lock _nextLock = new();

        private T[] _array;
        private SmartMemoryPoolAllocationPart _child;
        private volatile SmartMemoryPoolAllocation? _next;

        public Memory<T> Memory { get; private set; }
        public SmartMemoryPoolAllocation(int size)
        {
            _array = new T[BitOperations.RoundUpToPowerOf2((uint)size)];
            Memory = new(_array);
            _child = new SmartMemoryPoolAllocationPart(this, 0, Memory.Length);
        }

        public SmartMemoryOwner Rent(int size)
        {
            return TryRentFromChild(size, out var owner)
                ? owner
                : RentFromNext(size);
        }

        private SmartMemoryOwner RentFromNext(int size)
        {
            if (_next is null)
            {
                lock (_nextLock)
                {
                    _next ??= new(size);
                }
            }

            return _next.Rent(size);
        }

        private bool TryRentFromChild(int size, [NotNullWhen(true)] out SmartMemoryOwner? owner)
        {
            return _child.TryRent(size, out owner);
        }

        public void Dispose()
        {
            var next = _next;
            DisposeCore();
            while (next is not null)
            {
                var newNext = next._next;
                next.DisposeCore();
                next = newNext;
            }
        }

        private void DisposeCore()
        {
            if (!Interlocked.CompareExchange(ref _disposed, true, false))
            {
                _array = null!;
                Memory = null;
                _child.Dispose();
                _child = null!;
                _next = null;
                _nextLock = null!;
            }
            else
            {
                throw new ObjectDisposedException(nameof(SmartMemoryPoolAllocation));
            }
        }
    }

    internal struct SmartMemoryPoolAllocationPartRange(int start, int end)
    {
        public int Start = start;
        public int End = end;

        public static ref long ToLong(ref SmartMemoryPoolAllocationPartRange range) =>
            ref Unsafe.As<SmartMemoryPoolAllocationPartRange, long>(ref range);
        public static long ToLong(SmartMemoryPoolAllocationPartRange range) =>
            ToLong(ref range);
        public static ref SmartMemoryPoolAllocationPartRange FromLong(ref long value) =>
            ref Unsafe.As<long, SmartMemoryPoolAllocationPartRange>(ref value);
        public static SmartMemoryPoolAllocationPartRange FromLong(long value) =>
            FromLong(ref value);
    }

    internal sealed class SmartMemoryPoolAllocationPart : CriticalFinalizerObject, IDisposable
    {
        private bool _disposed;
        private volatile SmartMemoryPoolAllocationPart? _next;
        private long _rangeLong;
        private volatile bool _used;
        private readonly SmartMemoryPoolAllocation _allocation;

        public SmartMemoryPoolAllocationPart(SmartMemoryPoolAllocation allocation, int start, int end, SmartMemoryPoolAllocationPart? next = default)
        {
            _allocation = allocation;
            Range = new(start, end);
            Memory = allocation.Memory[Range.Start..Range.End];
            _next = next;
        }

        public Memory<T> Memory { get; private set; }

        public bool IsUsed => _used;

        public ref SmartMemoryPoolAllocationPartRange Range =>
            ref SmartMemoryPoolAllocationPartRange.FromLong(ref _rangeLong);

        public SmartMemoryPoolAllocationPartRange RangeCopy =>
            SmartMemoryPoolAllocationPartRange.FromLong(Interlocked.Read(ref _rangeLong));

        public int Size
        {
            get
            {
                var copy = RangeCopy;
                return copy.End - copy.Start;
            }
        }

        public bool TryRent(int size, [NotNullWhen(true)] out SmartMemoryOwner? owner)
        {
            if (TryRentFromCurrent(size, out owner) || TryRentFromNext(size, out owner))
            {
                return true;
            }

            owner = null;
            return false;
        }

        public bool TryRentFromNext(int size, [NotNullWhen(true)] out SmartMemoryOwner? owner)
        {
            if (_next is null)
            {
                owner = null;
                return false;
            }

            return _next.TryRent(size, out owner);
        }

        public bool TryRentFromCurrent(int size, [NotNullWhen(true)] out SmartMemoryOwner? owner)
        {
            if (!TryCheckMerge(size))
            {
                owner = null;
                return false;
            }

            var endUsed = Range.Start + size;
            if (Range.End != endUsed)
            {
                var next = new SmartMemoryPoolAllocationPart(_allocation, endUsed, Range.End, _next);
                Range.End = endUsed;
                _next = next;
            }
            Memory = _allocation.Memory[Range.Start..Range.End];
            owner = new(this);
            return true;
        }

        public bool TryCheckMerge(int size)
        {
            if (!TryUsed())
            {
                return false;
            }

            if (Size >= size)
            {
                return true;
            }

            if (_next?.TryCheckMerge(size - Size) == true)
            {
                Range.End = _next.Range.End;
                _next = _next._next;
                return true;
            }

            UnUsed();
            return false;
        }

        public bool TryUsed()
        {
            return !Interlocked.CompareExchange(ref _used, true, false);
        }

        public void UnUsed()
        {
            Interlocked.Exchange(ref _used, false);
        }

        public void Dispose()
        {
            var next = _next;
            DisposeCore();
            while (next is not null)
            {
                var newNext = next._next;
                next.DisposeCore();
                next = newNext;
            }
        }

        private void DisposeCore()
        {
            if (!Interlocked.CompareExchange(ref _disposed, true, false))
            {
                _next = null;
            }
            else
            {
                throw new ObjectDisposedException(nameof(SmartMemoryPoolAllocationPart));
            }
        }
    }


    internal sealed class SmartMemoryOwner : CriticalFinalizerObject, IMemoryOwner<T>
    {
        private bool _disposed;
        private readonly SmartMemoryPoolAllocationPart _part;

        internal SmartMemoryOwner(SmartMemoryPoolAllocationPart part)
        {
            _part = part;
        }

        public Memory<T> Memory => _part.Memory;

        ~SmartMemoryOwner()
        {
            DisposeCore();
        }

        public void Dispose()
        {
            if (!Interlocked.CompareExchange(ref _disposed, true, false))
            {
                DisposeCore();
                GC.SuppressFinalize(this);
            }
            else
            {
                throw new ObjectDisposedException(nameof(SmartMemoryOwner));
            }
        }

        private void DisposeCore()
        {
            _part.UnUsed();
        }
    }
}
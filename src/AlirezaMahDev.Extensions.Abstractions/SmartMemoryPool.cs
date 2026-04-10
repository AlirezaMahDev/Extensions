using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.ConstrainedExecution;

namespace AlirezaMahDev.Extensions.Abstractions;

public sealed class SmartMemoryPool
{
    public const int MinSize = byte.MaxValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static SmartMemoryPool<T> GetShared<T>() => SmartMemoryPool<T>.Shared;
}

public sealed class SmartMemoryPool<T> : MemoryPool<T>
{
    public override int MaxBufferSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => int.MaxValue;
    }

    public static new SmartMemoryPool<T> Shared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = new();

    private readonly SmartMemoryPoolAllocation<T> _allocation = new(SmartMemoryPool.MinSize);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override IMemoryOwner<T> Rent(int size = -1)
    {
        return _allocation.Rent(size == -1 ? SmartMemoryPool.MinSize : size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _allocation.Dispose();
        }
    }

}

internal sealed class SmartMemoryPoolAllocation<T> : CriticalFinalizerObject, IDisposable
{
    private bool _disposed;
    private Lock _nextLock = new();

    private T[] _array;
    private volatile SmartMemoryPoolAllocation<T>? _next;
    public ConcurrencyNativeRefList<SmartMemoryPoolAllocationPart<T>> Parts;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SmartMemoryPoolAllocation(int size)
    {
        _array = GC.AllocateUninitializedArray<T>((int)BitOperations.RoundUpToPowerOf2((uint)size), true);
        Memory = new(_array);
        Parts = new(size);
        Parts.Add(new SmartMemoryPoolAllocationPart<T>(0, Memory.Length));
    }

    public Memory<T> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private set;
    }

    private ref SmartMemoryPoolAllocationPart<T> Child
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Parts[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SmartMemoryOwner<T> Rent(int size)
    {
        return TryRentFromChild(size, out var owner)
            ? owner
            : RentFromNext(size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private SmartMemoryOwner<T> RentFromNext(int size)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool TryRentFromChild(int size, [NotNullWhen(true)] out SmartMemoryOwner<T>? owner)
    {
        int index = 0;
        while (index != -1)
        {
            ref var current = ref Parts[index];
            if (current.TryRent(this, size))
            {
                owner = new(this, index);
                return true;
            }

            index = current.Next;
        }

        owner = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void DisposeCore()
    {
        if (!Interlocked.CompareExchange(ref _disposed, true, false))
        {
            _array = null!;
            Memory = null;
            Parts.Dispose();
            _next = null;
            _nextLock = null!;
        }
        else
        {
            throw new ObjectDisposedException(nameof(SmartMemoryPoolAllocation<T>));
        }
    }
}

internal struct SmartMemoryPoolAllocationPartRange(int start, int end)
{
    public readonly int Start = start;
    public int End = end;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ref long ToLong(ref SmartMemoryPoolAllocationPartRange range) =>
        ref Unsafe.As<SmartMemoryPoolAllocationPartRange, long>(ref range);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static long ToLong(SmartMemoryPoolAllocationPartRange range) =>
        ToLong(ref range);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ref SmartMemoryPoolAllocationPartRange FromLong(ref long value) =>
        ref Unsafe.As<long, SmartMemoryPoolAllocationPartRange>(ref value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static SmartMemoryPoolAllocationPartRange FromLong(long value) =>
        FromLong(ref value);
}

internal struct SmartMemoryPoolAllocationPart<T>
{
    public volatile int Next;
    public long RangeLong;
    public volatile bool Used;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SmartMemoryPoolAllocationPart(int start, int end, int next = -1)
    {
        var range = new SmartMemoryPoolAllocationPartRange(start, end);
        RangeLong = SmartMemoryPoolAllocationPartRange.ToLong(ref range);
        Next = next;
    }
}

internal static class SmartMemoryPoolAllocationPartExtensions
{
    extension<T>(ref SmartMemoryPoolAllocationPart<T> part)
    {

        private ref SmartMemoryPoolAllocationPartRange Range
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => ref SmartMemoryPoolAllocationPartRange.FromLong(ref part.RangeLong);
        }

        private SmartMemoryPoolAllocationPartRange RangeCopy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => SmartMemoryPoolAllocationPartRange.FromLong(Interlocked.Read(ref part.RangeLong));
        }

        private int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                var copy = part.RangeCopy;
                return copy.End - copy.Start;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private ref SmartMemoryPoolAllocationPart<T> GetNext(SmartMemoryPoolAllocation<T> allocation)
        {
            return ref part.Next is var next && next != -1
                ? ref allocation.Parts[next]
                : ref Unsafe.NullRef<SmartMemoryPoolAllocationPart<T>>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Memory<T> GetMemory(SmartMemoryPoolAllocation<T> allocation)
        {
            var range = part.RangeCopy;
            return allocation.Memory[range.Start..range.End];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryRent(SmartMemoryPoolAllocation<T> allocation, int size)
        {
            if (!part.TryCheckMerge(allocation, size))
            {
                return false;
            }

            var endUsed = part.Range.Start + size;
            if (part.Range.End != endUsed)
            {
                var next = new SmartMemoryPoolAllocationPart<T>(endUsed, part.Range.End, part.Next);
                part.Range.End = endUsed;
                part.Next = allocation.Parts.Add(next);
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private bool TryCheckMerge(SmartMemoryPoolAllocation<T> allocation, int size)
        {
            if (!part.TryUsed())
            {
                return false;
            }

            if (part.Size >= size)
            {
                return true;
            }

            ref var next = ref part.GetNext(allocation);
            if (!Unsafe.IsNullRef(ref next) && next.TryCheckMerge(allocation, size - part.Size))
            {
                part.Range.End = next.Range.End;
                part.Next = next.Next;
                return true;
            }

            part.UnUsed();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private bool TryUsed()
        {
            return !Interlocked.CompareExchange(ref part.Used, true, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void UnUsed()
        {
            Interlocked.Exchange(ref part.Used, false);
        }
    }
}


internal sealed class SmartMemoryOwner<T> : CriticalFinalizerObject, IMemoryOwner<T>
{
    private bool _disposed;
    private readonly SmartMemoryPoolAllocation<T> _allocation;
    private readonly int _part;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal SmartMemoryOwner(SmartMemoryPoolAllocation<T> allocation, int part)
    {
        _allocation = allocation;
        _part = part;
        Memory = _allocation.Parts[part].GetMemory(_allocation);
    }

    public Memory<T> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    ~SmartMemoryOwner()
    {
        DisposeCore();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (!Interlocked.CompareExchange(ref _disposed, true, false))
        {
            DisposeCore();
            GC.SuppressFinalize(this);
        }
        else
        {
            throw new ObjectDisposedException(nameof(SmartMemoryOwner<T>));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void DisposeCore()
    {
        Memory.Span.Clear();
        _allocation.Parts[_part].UnUsed();
    }
}
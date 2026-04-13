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
    public static new SmartMemoryPool<T> Shared { get; } = new();

    public override int MaxBufferSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => int.MaxValue;
    }
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

internal sealed class SmartMemoryPoolAllocation<T> : IDisposable
{
    private bool _disposed;
    private Lock _nextLock = new();

    private T[] _array;
    private volatile SmartMemoryPoolAllocation<T>? _next;
    public NativeConcurrencyRefPool<SmartMemoryPoolAllocationPart<T>> PartsPool;
    public NativeConcurrencyRefBag<ConcurrencyIndex> PartsUnUsed;
    private readonly int _size;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SmartMemoryPoolAllocation(int size)
    {
        _array = GC.AllocateUninitializedArray<T>((int)BitOperations.RoundUpToPowerOf2((uint)size));
        Memory = new(_array);
        PartsPool = NativeConcurrencyRefPool<SmartMemoryPoolAllocationPart<T>>.Create();
        PartsUnUsed = NativeConcurrencyRefBag<ConcurrencyIndex>.Create();
        PartsUnUsed.TryAdd(PartsPool.Rent(new SmartMemoryPoolAllocationPart<T>(0, Memory.Length, ConcurrencyIndex.Null)));
        _size = size;
    }

    public Memory<T> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SmartMemoryOwner<T> Rent(int size)
    {
        var allocation = this;

        SmartMemoryOwner<T>? owner;
        while (!allocation.TryRent(size, out owner))
        {
            if (allocation._next is null)
            {
                lock (allocation._nextLock)
                {
                    allocation._next ??= new(_size + 1);
                }
            }
            allocation = allocation._next;
        }
        return owner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool TryRent(int size, [NotNullWhen(true)] out SmartMemoryOwner<T>? owner)
    {
        if (size > _size)
        {
            owner = null;
            return false;
        }

        for (int i = 0; i < PartsUnUsed.Length; i++)
        {
            if (PartsUnUsed.TryRemove(out var index))
            {
                using LockRefItem<SmartMemoryPoolAllocationPart<T>> @lock = PartsPool[index];
                ref var current = ref @lock.Value;
                if (current.TryRent(this, size))
                {
                    owner = new(this, index);
                    return true;
                }
                else
                {
                    PartsUnUsed.TryAdd(index);
                }
            }
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
            PartsPool.Dispose();
            _next = null;
            _nextLock = null!;
        }
        else
        {
            throw new ObjectDisposedException(nameof(SmartMemoryPoolAllocation<>));
        }
    }
}

internal struct SmartMemoryPoolAllocationPartRange(int start, int end)
{
    public int Start = start;
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

[StructLayout(LayoutKind.Sequential, Size = 16)]
internal struct SmartMemoryPoolAllocationPart<T>
{
    public long Range;
    public long Next;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SmartMemoryPoolAllocationPart(int start, int end, ConcurrencyIndex next)
    {
        var range = new SmartMemoryPoolAllocationPartRange(start, end);
        Range = SmartMemoryPoolAllocationPartRange.ToLong(new SmartMemoryPoolAllocationPartRange(start, end));
        Next = next.ToRefLong();
    }
}

internal static class SmartMemoryPoolAllocationPartExtensions
{
    extension<T>(ref SmartMemoryPoolAllocationPart<T> part)
    {
        private ref SmartMemoryPoolAllocationPartRange PartRange
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => ref SmartMemoryPoolAllocationPartRange.FromLong(ref part.Range);
        }

        private SmartMemoryPoolAllocationPartRange PartRangeCopy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => SmartMemoryPoolAllocationPartRange.FromLong(Volatile.Read(ref part.Range));
        }

        private int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                var copy = part.PartRangeCopy;
                return copy.End - copy.Start;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Memory<T> GetMemory(SmartMemoryPoolAllocation<T> allocation)
        {
            var range = part.PartRangeCopy;
            return allocation.Memory[range.Start..range.End];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryRent(SmartMemoryPoolAllocation<T> allocation, int size)
        {
            if (part.Size < size)
            {
                bool result = false;
                while (part.Next != ConcurrencyIndex.Null.ToRefLong()
                        && allocation.PartsUnUsed.TryRemove(
                            ConcurrencyIndex.FromRefLong(ref part.Next),
                            out var nextIndex))
                {

                    using var next = allocation.PartsPool[nextIndex];
                    part.PartRange.End = next.Value.PartRange.End;
                    part.Next = next.Value.Next;
                    allocation.PartsPool.Return(nextIndex);
                    if (part.Size >= size)
                    {
                        result = true;
                        break;
                    }
                }

                if (!result)
                {
                    return false;
                }
            }

            var endPart = part.PartRange.End;
            var startPart = part.PartRange.End = part.PartRange.Start + size;

            long lastNext;
            var newNext = allocation.PartsPool.Rent();
            using var newNextValue = newNext.Value;
            do
            {
                lastNext = Volatile.Read(ref part.Next);
                newNextValue.Value = new SmartMemoryPoolAllocationPart<T>(
                    startPart,
                    endPart,
                    ConcurrencyIndex.FromRefLong(ref lastNext)
                );
            } while (
                Interlocked.CompareExchange(
                ref part.Next,
                newNext.Index.ToRefLong(),
                lastNext) != lastNext
            );
            allocation.PartsUnUsed.TryAdd(newNext.Index);

            return true;
        }
    }
}


internal sealed class SmartMemoryOwner<T> : IMemoryOwner<T>
{
    private bool _disposed;
    private readonly SmartMemoryPoolAllocation<T> _allocation;
    private readonly ConcurrencyIndex _partIndex;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal SmartMemoryOwner(SmartMemoryPoolAllocation<T> allocation, ConcurrencyIndex part)
    {
        _allocation = allocation;
        _partIndex = part;
        using var concurrencyRefItem = _allocation.PartsPool[part];
        Memory = concurrencyRefItem.Value.GetMemory(_allocation);
    }

    public Memory<T> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
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
            throw new ObjectDisposedException(nameof(SmartMemoryOwner<>));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void DisposeCore()
    {
        Memory.Span.Clear();
        _allocation.PartsUnUsed.TryAdd(_partIndex);
    }
}

using System.Buffers;

namespace AlirezaMahDev.Extensions.Abstractions;

internal sealed class SmartMemoryOwner<T>(int id) : IMemoryOwner<T>
{
    private static readonly NativeConcurrencyRefBag<int> Bag = NativeConcurrencyRefBag<int>.Create();
    private static readonly SmartMemoryOwner<T>[] Array
        = [
            .. Enumerable
                .Range(0,Environment.ProcessorCount * 256)
                .Select(static x => {
                    Bag.TryAdd(x);
                    return new SmartMemoryOwner<T>(x);
                })
        ];

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static SmartMemoryOwner<T> RentOwner()
    {
        return Bag.TryRemove(out var index)
            ? Array[index]
            : new(-1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static void ReturnOwner(SmartMemoryOwner<T> owner)
    {
        if (owner.Id != -1)
        {
            Bag.TryAdd(owner.Id);
        }
    }

    internal int Id
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = id;

    private bool _disposed;
    private SmartMemoryPoolAllocation<T>? _allocation;
    private ConcurrencyIndex _index;

    public Memory<T> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal set;
    } = new([]);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal void Initializer(SmartMemoryPoolAllocation<T> allocation, ConcurrencyIndex index, int size)
    {
        _disposed = false;
        _allocation = allocation;
        _index = index;
        Memory = _allocation.Array[index.ShardingIndex]
            .AsMemory(allocation.Size * index.ShardingItemIndex, allocation.Size)[..size];
        if (allocation.Pool.ClearOnRent)
        {
            Memory.Span.Clear();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (_allocation is null) { return; }
        if (!Interlocked.CompareExchange(ref _disposed, true, false))
        {
            _allocation.UnUsed.TryAdd(_index);
            _allocation = null!;
            SmartMemoryOwner<T>.ReturnOwner(this);
        }
        else
        {
            throw new ObjectDisposedException(nameof(SmartMemoryOwner<>));
        }
    }
}
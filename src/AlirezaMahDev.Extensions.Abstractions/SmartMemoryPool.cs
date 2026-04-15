using System.Buffers;
using System.Collections.Concurrent;
using System.Numerics;

namespace AlirezaMahDev.Extensions.Abstractions;

public sealed class SmartMemoryPool
{
    public const int MinSize = 1;
    public const int MinCount = 1 << 4;
    public const int MaxBufferSizeBit = 24;
    public const int MaxSegments = 64;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static SmartMemoryPool<T> GetShared<T>() => SmartMemoryPool<T>.Shared;
}

public sealed class SmartMemoryPool<T> : MemoryPool<T>
{
    public static SmartMemoryPool<T> SharedNoClear
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = new();
    public static new SmartMemoryPool<T> Shared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = new(true);

    public override int MaxBufferSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => 1 << SmartMemoryPool.MaxBufferSizeBit;
    }

    public bool ClearOnRent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    private readonly Lazy<SmartMemoryPoolAllocation<T>>[] _allocations;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SmartMemoryPool(bool clearOnRent = false)
    {
        _allocations = new Lazy<SmartMemoryPoolAllocation<T>>[SmartMemoryPool.MaxBufferSizeBit + 1];
        for (int i = 0; i < _allocations.Length; i++)
        {
            var captured = i;
            _allocations[captured] = new(() =>
                new SmartMemoryPoolAllocation<T>(this, 1 << captured), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        ClearOnRent = clearOnRent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override IMemoryOwner<T> Rent(int size = -1)
    {
        if (size == -1)
        {
            size = SmartMemoryPool.MinSize;
        }
        return size == 0
            ? new SmartMemoryOwner<T>(-1)
            : _allocations[BitOperations.Log2(BitOperations.RoundUpToPowerOf2((uint)size))].Value.Rent(size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var allocation in _allocations.Where(x => x.IsValueCreated))
            {
                allocation.Value.Dispose();
            }
        }
    }
}
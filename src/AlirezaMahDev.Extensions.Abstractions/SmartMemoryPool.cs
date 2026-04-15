using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
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

internal sealed class SmartMemoryPoolAllocation<T> : IDisposable
{
    private volatile bool _growing;
    private volatile int _lastArrayInitialized = -1;
    public T[][] Array;
    public NativeConcurrencyRefBag<ConcurrencyIndex> UnUsed;

    public SmartMemoryPool<T> Pool
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
    public int Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
    public volatile int Count = SmartMemoryPool.MinCount - 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SmartMemoryPoolAllocation(SmartMemoryPool<T> pool, int size)
    {
        Pool = pool;
        Size = size;
        Array = new T[SmartMemoryPool.MaxSegments][];
        UnUsed = NativeConcurrencyRefBag<ConcurrencyIndex>.Create();
        Grow();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool Grow()
    {
        if (!Interlocked.CompareExchange(ref _growing, true, false))
        {
            try
            {
                _lastArrayInitialized++;
                var newCount = (int)BitOperations.RoundUpToPowerOf2((uint)Count + 1);
                Count += newCount;
                Array[_lastArrayInitialized] = GC.AllocateUninitializedArray<T>(Size * newCount);
                for (int i = 0; i < newCount; i++)
                {
                    UnUsed.TryAdd(new ConcurrencyIndex(_lastArrayInitialized, i));
                }

                return true;
            }
            finally
            {
                Interlocked.Exchange(ref _growing, false);
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public SmartMemoryOwner<T> Rent(int size)
    {
        SpinWait spinWait = default;
        SmartMemoryOwner<T>? owner;
        while (!TryRent(size, out owner))
        {
            if (!Grow())
            {
                spinWait.SpinOnce();
            }
        }
        return owner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool TryRent(int size, [NotNullWhen(true)] out SmartMemoryOwner<T>? owner)
    {
        if (UnUsed.TryRemove(out var index))
        {
            owner = SmartMemoryOwner<T>.RentOwner();
            owner.Initializer(this, index, size);
            return true;
        }

        owner = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        UnUsed.Dispose();
        Array = null!;
    }
}


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

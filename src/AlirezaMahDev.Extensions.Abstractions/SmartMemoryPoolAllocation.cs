using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace AlirezaMahDev.Extensions.Abstractions;

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
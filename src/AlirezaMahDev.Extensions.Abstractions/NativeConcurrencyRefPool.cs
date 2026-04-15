namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public struct NativeConcurrencyRefPool<T>(int capacity, bool init) : IConcurrencyRefPool<NativeConcurrencyRefPool<T>, T>, IDisposable
    where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefPool<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    private readonly NativeConcurrencyRefBag<ConcurrencyIndex> _free = new(capacity, init);
    private readonly NativeConcurrencyRefBag<T> _used = new(capacity, init);

    public readonly LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _used[index];
        }
    }

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _used.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly ConcurrencyRefIndexableItem<NativeConcurrencyRefPool<T>, T> Rent()
    {
        return _free.TryRemove(out var item)
            ? new(this, item)
            : new(this, _used.TryAdd(default(T)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly void Return(ConcurrencyIndex index)
    {
        _free.TryAdd(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly ConcurrencyRefIndexableEnumerator<NativeConcurrencyRefPool<T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly int GetShardingLength()
    {
        return _used.GetShardingLength();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly int GetShardingItemsLength(int shardingIndex)
    {
        return _used.GetShardingItemsLength(shardingIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly void Clean()
    {
        _free.Clean();
        _used.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly void Dispose()
    {
        _free.Dispose();
        _used.Dispose();
    }
}
namespace AlirezaMahDev.Extensions.Abstractions;

public interface IConcurrencyRefIndexable<TSelf, T> : ILockRefEnumerable<TSelf, T, ConcurrencyRefIndexableEnumerator<TSelf, T>>, IRefLength
    where TSelf : IConcurrencyRefIndexable<TSelf, T>, allows ref struct
{
    int GetShardingLength();
    int GetShardingItemsLength(int shardingIndex);

    LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}
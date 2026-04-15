namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct ConcurrencyRefIndexableEnumerator<TSelf, T>(TSelf self) : ILockRefEnumerator<ConcurrencyRefIndexableEnumerator<TSelf, T>, T>
    where TSelf : IConcurrencyRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _shardingIndex = 0;
    private int _shardingItemIndex = -1;
    private readonly ConcurrencyIndex Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return new(_shardingIndex, _shardingItemIndex);
        }
    }

    public readonly LockRefItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _self[Index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        while (true)
        {
            _shardingItemIndex++;
            if (_shardingItemIndex < _self.GetShardingItemsLength(_shardingIndex))
            {
                break;
            }

            _shardingIndex++;

            if (_shardingIndex < _self.GetShardingLength())
            {
                _shardingItemIndex = -1;
                continue;
            }

            return false;
        }

        return true;
    }
}
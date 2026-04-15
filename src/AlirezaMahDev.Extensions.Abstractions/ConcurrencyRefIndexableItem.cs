namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct ConcurrencyRefIndexableItem<TSelf, T>(TSelf self, ConcurrencyIndex index)
    where TSelf : IConcurrencyRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    public ConcurrencyIndex Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = index;
    public LockRefItem<T> Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _self[Index];
        }
    }
}
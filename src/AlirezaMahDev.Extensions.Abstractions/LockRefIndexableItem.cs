namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct LockRefIndexableItem<TSelf, T>(TSelf self, int index)
    where TSelf : ILockRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    public int Index
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
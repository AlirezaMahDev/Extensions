namespace AlirezaMahDev.Extensions.Abstractions;

public readonly ref struct RefIndexableItem<TSelf, T>(TSelf self, int index)
    where TSelf : IRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;

    public int Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = index;

    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _self[Index];
        }
    }
}
namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataCollectionWrap<TValue, TItem, TItemWrap>(
    ScopedRefValueFunc<TValue, DataOffset> refChild,
    ScopedRefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyChild,
    TItemWrap itemWrap)
    where TValue : unmanaged, IDataValue<TValue>
    where TItem : unmanaged, IDataValue<TItem>
    where TItemWrap : DataCollectionItemWrap<TItem>
{
    public ScopedRefValueFunc<TValue, DataOffset> RefChild
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = refChild;

    public ScopedRefReadOnlyValueFunc<TValue, DataOffset> RefReadOnlyChild
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = refReadOnlyChild;

    public TItemWrap ItemWrap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = itemWrap;
}

public class DataCollectionWrap<TValue, TItem>(
    ScopedRefValueFunc<TValue, DataOffset> refChild,
    ScopedRefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyChild,
    ScopedRefValueFunc<TItem, DataOffset> refNext,
    ScopedRefReadOnlyValueFunc<TItem, DataOffset> refReadOnlyNext)
        : DataCollectionWrap<TValue, TItem, DataCollectionItemWrap<TItem>>(refChild, refReadOnlyChild,
        new(refNext, refReadOnlyNext))
    where TValue : unmanaged, IDataValue<TValue>
    where TItem : unmanaged, IDataValue<TItem>
{
}

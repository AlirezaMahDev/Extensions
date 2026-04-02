namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataCollectionWrap<TValue, TItem, TItemWrap>(
    RefValueFunc<TValue, DataOffset> refChild,
    RefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyChild,
    TItemWrap itemWrap)
    where TValue : unmanaged, IDataValue<TValue>
    where TItem : unmanaged, IDataValue<TItem>
    where TItemWrap : DataCollectionItemWrap<TItem>
{
    public RefValueFunc<TValue, DataOffset> RefChild
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = refChild;

    public RefReadOnlyValueFunc<TValue, DataOffset> RefReadOnlyChild
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
    RefValueFunc<TValue, DataOffset> refChild,
    RefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyChild,
    RefValueFunc<TItem, DataOffset> refNext,
    RefReadOnlyValueFunc<TItem, DataOffset> refReadOnlyNext)
        : DataCollectionWrap<TValue, TItem, DataCollectionItemWrap<TItem>>(refChild, refReadOnlyChild,
        new(refNext, refReadOnlyNext))
    where TValue : unmanaged, IDataValue<TValue>
    where TItem : unmanaged, IDataValue<TItem>
{
}

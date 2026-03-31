namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataCollectionWrapFullDefault<TValue, TItem>
    where TValue : unmanaged, IDataCollection<TValue>
    where TItem : unmanaged, IDataCollectionItem<TItem>
{
    public static readonly DataCollectionWrap<TValue, TItem> Default
        = new(DataCollectionWrapChildDefault<TValue>.RefChild,
            DataCollectionWrapChildDefault<TValue>.RefReadOnlyChild,
            DataCollectionWrapNextDefault<TItem>.RefNext,
            DataCollectionWrapNextDefault<TItem>.RefReadOnlyNext);
}
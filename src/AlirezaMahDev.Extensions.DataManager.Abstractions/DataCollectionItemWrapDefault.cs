namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataCollectionItemWrapDefault<TValue, TItem>
    where TValue : unmanaged, IDataCollectionItem<TValue>
    where TItem : unmanaged, IDataCollectionItem<TItem>
{
    public static readonly DataCollectionItemWrap<TValue> Default
        = new(DataCollectionWrapNextDefault<TValue>.RefNext,
            DataCollectionWrapNextDefault<TValue>.RefReadOnlyNext);
}
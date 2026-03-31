namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataDictionaryWrapFullDefault<TValue, TItem, TKey>
    where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
    where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>
    where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
{
    public static readonly DataDictionaryWrap<TValue, TItem, TKey> Default
        = new(DataCollectionWrapChildDefault<TValue>.RefChild,
            DataCollectionWrapChildDefault<TValue>.RefReadOnlyChild,
            DataCollectionWrapNextDefault<TItem>.RefNext,
            DataCollectionWrapNextDefault<TItem>.RefReadOnlyNext,
            DataDictionaryWrapKeyDefault<TItem, TKey>.RefKey,
            DataDictionaryWrapKeyDefault<TItem, TKey>.RefReadOnlyKey);
}
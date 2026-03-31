namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataDictionaryItemWrapDefault<TValue, TKey>
    where TValue : unmanaged, IDataDictionaryItem<TValue, TKey>
    where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
{
    public static readonly DataDictionaryItemWrap<TValue, TKey> Default
        = new(DataCollectionWrapNextDefault<TValue>.RefNext,
            DataCollectionWrapNextDefault<TValue>.RefReadOnlyNext,
            DataDictionaryWrapKeyDefault<TValue, TKey>.RefKey,
            DataDictionaryWrapKeyDefault<TValue, TKey>.RefReadOnlyKey);
}
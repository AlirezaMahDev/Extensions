namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataDictionaryItemWrap<TValue, TKey>(
    RefValueFunc<TValue, DataOffset> getRefNext,
    RefReadOnlyValueFunc<TValue, DataOffset> getRefReadOnlyNext,
    RefValueFunc<TValue, TKey> getRefKey,
    RefReadOnlyValueFunc<TValue, TKey> getRefReadOnlyKey) : DataCollectionItemWrap<TValue>(getRefNext, getRefReadOnlyNext)
    where TValue : unmanaged, IDataValue<TValue>
{
    public RefReadOnlyValueFunc<TValue, TKey> RefReadOnlyKey
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = getRefReadOnlyKey;

    public RefValueFunc<TValue, TKey> RefKey
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = getRefKey;
}
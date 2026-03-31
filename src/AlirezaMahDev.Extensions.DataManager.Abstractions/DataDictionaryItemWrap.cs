namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataDictionaryItemWrap<TValue, TKey>(
    ScopedRefValueFunc<TValue, DataOffset> getRefNext,
    ScopedRefReadOnlyValueFunc<TValue, DataOffset> getRefReadOnlyNext,
    ScopedRefValueFunc<TValue, TKey> getRefKey,
    ScopedRefReadOnlyValueFunc<TValue, TKey> getRefReadOnlyKey) : DataCollectionItemWrap<TValue>(getRefNext, getRefReadOnlyNext)
    where TValue : unmanaged, IDataValue<TValue>
{
    public ScopedRefReadOnlyValueFunc<TValue, TKey> RefReadOnlyKey
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = getRefReadOnlyKey;

    public ScopedRefValueFunc<TValue, TKey> RefKey
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = getRefKey;
}
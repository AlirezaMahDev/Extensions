namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataDictionaryWrapKeyDefault<TValue, TKey>
    where TValue : unmanaged, IDataDictionaryItem<TValue, TKey>
    where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
{
    public static readonly ScopedRefValueFunc<TValue, TKey> RefKey = (scoped ref value) =>
        ref value.Key;
    public static readonly ScopedRefReadOnlyValueFunc<TValue, TKey> RefReadOnlyKey = (scoped ref readonly value) =>
        ref value.Key;
}
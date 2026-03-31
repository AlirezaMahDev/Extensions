namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataDictionaryItem<TValue> : IDataCollectionItem<TValue>
    where TValue : unmanaged, IDataDictionaryItem<TValue>;

public interface IDataDictionaryItem<TValue, TKey> : IDataDictionaryItem<TValue>
    where TValue : unmanaged, IDataDictionaryItem<TValue, TKey>
    where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
{
    ref TKey Key { get; }
}
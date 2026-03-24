namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataDictionaryItem<TValue, TKey> : IDataCollectionItem<TValue>
    where TValue : unmanaged, IDataDictionaryItem<TValue, TKey>
    where TKey : unmanaged, IEquatable<TKey>
{
    ref TKey Key { get; }
}
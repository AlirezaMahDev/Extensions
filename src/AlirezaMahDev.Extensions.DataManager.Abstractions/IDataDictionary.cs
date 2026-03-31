namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataDictionary<TValue> : IDataCollection<TValue>
    where TValue : unmanaged, IDataDictionary<TValue>;

public interface IDataDictionary<TValue, TItem> : IDataDictionary<TValue>, IDataCollection<TValue, TItem>
    where TValue : unmanaged, IDataDictionary<TValue, TItem>
    where TItem : unmanaged, IDataDictionaryItem<TItem>;

public interface IDataDictionary<TValue, TItem, TKey> : IDataDictionary<TValue, TItem>
    where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
    where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>
    where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>;
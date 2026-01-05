namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataCollection<TValue> : IDataLock<TValue>
    where TValue : unmanaged, IDataCollection<TValue>
{
    DataOffset Child { get; set; }
}

public interface IDataCollection<TValue, TItem> : IDataCollection<TValue>
    where TValue : unmanaged, IDataCollection<TValue, TItem>
    where TItem : unmanaged, IDataCollectionItem<TItem>;
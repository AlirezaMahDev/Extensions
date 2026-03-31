namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataCollection<TValue> : IDataValue<TValue>
    where TValue : unmanaged, IDataCollection<TValue>
{
    ref DataOffset Child { get; }
}

public interface IDataCollection<TValue, TItem> : IDataCollection<TValue>
    where TValue : unmanaged, IDataCollection<TValue, TItem>
    where TItem : unmanaged, IDataCollectionItem<TItem>;
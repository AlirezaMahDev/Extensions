namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataCollectionItem<TValue> : IDataLock<TValue>
    where TValue : unmanaged, IDataCollectionItem<TValue>
{
    DataOffset Next { get; set; }
}
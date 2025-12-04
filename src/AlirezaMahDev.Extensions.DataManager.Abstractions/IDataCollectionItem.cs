namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataCollectionItem<TValue> : IDataValue<TValue>
    where TValue : unmanaged, IDataCollectionItem<TValue>
{
    long Next { get; set; }
}
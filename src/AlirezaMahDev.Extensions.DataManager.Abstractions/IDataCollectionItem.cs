namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataCollectionItem<TValue> : IDataLock<TValue>
    where TValue : unmanaged, IDataCollectionItem<TValue>
{
    ref DataOffset Next { get;  }
}
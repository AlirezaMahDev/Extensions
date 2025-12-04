namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataCollectionTree<TValue> : IDataCollection<TValue, TValue>, IDataCollectionItem<TValue>
    where TValue : unmanaged, IDataCollectionTree<TValue>;
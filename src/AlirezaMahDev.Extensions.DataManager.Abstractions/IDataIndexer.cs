namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataIndexer<TSelf> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>;
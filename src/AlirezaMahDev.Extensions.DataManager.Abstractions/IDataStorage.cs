namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataStorage<TSelf> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>
{
    ref DataOffset Data { get; }
}

public interface IDataStorage<TSelf, TDataValue> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>
    where TDataValue : unmanaged, IDataValue<TDataValue>
{
    ref DataOffset Data { get; }
}
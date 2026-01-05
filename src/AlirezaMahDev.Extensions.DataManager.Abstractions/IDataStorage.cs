namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataStorage<TSelf> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>
{
    DataOffset Data { get; set; }
}

public interface IDataStorage<TSelf, TDataValue> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>
    where TDataValue : unmanaged, IDataValue<TDataValue>
{
    DataOffset Data { get; set; }
}
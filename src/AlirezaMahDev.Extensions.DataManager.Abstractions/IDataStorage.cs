namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataStorage<TSelf> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>
{
    long Data { get; set; }
    int Size { get; set; }
}

public interface IDataStorage<TSelf, TDataValue> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>
    where TDataValue : unmanaged, IDataValue<TDataValue>
{
    long Data { get; set; }
}
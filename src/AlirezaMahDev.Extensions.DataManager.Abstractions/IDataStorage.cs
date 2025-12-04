namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataStorage<TSelf> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>
{
    long Data { get; set; }
    int Size { get; set; }
}
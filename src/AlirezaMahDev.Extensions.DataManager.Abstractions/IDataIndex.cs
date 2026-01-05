namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataIndex<TSelf> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>
{
    DataOffset Index { get; set; }
}
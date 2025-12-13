namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataIndex<TSelf> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataStorage<TSelf>
{
    long Index { get; set; }
}
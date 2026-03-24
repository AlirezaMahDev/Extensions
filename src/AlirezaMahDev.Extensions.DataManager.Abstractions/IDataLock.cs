namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataLock<TValue> : IDataValue<TValue>
    where TValue : unmanaged, IDataLock<TValue>
{
    ref int Lock { get; }
}
namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellValue<TSelf> : IDataLock<TSelf>
    where TSelf : unmanaged, IDataLock<TSelf>;
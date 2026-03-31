namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellValue<TSelf> : IDataValue<TSelf>
    where TSelf : unmanaged, IDataValue<TSelf>;
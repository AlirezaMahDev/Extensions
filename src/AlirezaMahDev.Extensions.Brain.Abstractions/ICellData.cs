namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellData<TSelf> : ICellBase<TSelf>
    where TSelf : unmanaged, ICellData<TSelf>;
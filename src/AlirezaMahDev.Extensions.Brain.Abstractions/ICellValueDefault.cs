using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellValueDefault<TSelf> : ICellValue<TSelf>, IDataValueDefault<TSelf>
    where TSelf : unmanaged, ICellValueDefault<TSelf>;
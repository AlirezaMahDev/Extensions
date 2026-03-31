namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate void DataLockScopedRefReadonlyAction<TValue>(scoped ref readonly TValue value)
    where TValue : unmanaged, IDataValue<TValue>;
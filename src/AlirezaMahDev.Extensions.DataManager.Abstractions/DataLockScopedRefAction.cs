namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate void DataLockScopedRefAction<TValue>(scoped ref TValue value)
    where TValue : unmanaged, IDataValue<TValue>;
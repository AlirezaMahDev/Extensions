namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate void DataLockScopedRefAction<TValue, TState>(scoped ref TValue value, scoped in TState state)
    where TValue : unmanaged, IDataValue<TValue>;

public delegate void DataLockScopedRefAction<TValue>(scoped ref TValue value)
    where TValue : unmanaged, IDataValue<TValue>;
namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate TResult DataLockScopedRefFunc<TValue, out TResult>(scoped ref TValue value)
    where TValue : unmanaged, IDataValue<TValue>;
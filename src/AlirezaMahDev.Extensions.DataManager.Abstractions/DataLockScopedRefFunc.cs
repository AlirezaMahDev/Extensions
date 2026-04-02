namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate TResult DataLockScopedRefFunc<TValue, TState, out TResult>(scoped ref TValue value,
    scoped in TState state)
    where TValue : unmanaged, IDataValue<TValue>;

public delegate TResult DataLockScopedRefFunc<TValue, out TResult>(scoped ref TValue value)
    where TValue : unmanaged, IDataValue<TValue>;
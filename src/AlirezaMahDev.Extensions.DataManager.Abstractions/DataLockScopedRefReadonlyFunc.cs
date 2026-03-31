namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate TResult DataLockScopedRefReadonlyFunc<TValue, out TResult>(scoped ref readonly TValue value)
    where TValue : unmanaged, IDataValue<TValue>;
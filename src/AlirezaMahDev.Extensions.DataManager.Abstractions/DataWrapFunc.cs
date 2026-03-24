namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate TResult DataWrapFunc<TValue, out TResult>(ref readonly DataWrap<TValue> wrap)
    where TValue : unmanaged, IDataValue<TValue>;
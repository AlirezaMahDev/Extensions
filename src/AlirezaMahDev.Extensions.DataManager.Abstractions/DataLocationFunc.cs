namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate TResult DataWrapFunc<TValue, out TResult>(DataWrap<TValue> wrap)
    where TValue : unmanaged, IDataValue<TValue>;
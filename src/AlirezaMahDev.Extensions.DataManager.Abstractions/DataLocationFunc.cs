namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate TResult DataLocationFunc<TValue, out TResult>(DataLocation<TValue> location)
    where TValue : unmanaged, IDataValue<TValue>;
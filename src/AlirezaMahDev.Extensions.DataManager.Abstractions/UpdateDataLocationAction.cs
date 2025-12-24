namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate void UpdateDataLocationAction<TValue>(DataLocation<TValue> location)
    where TValue : unmanaged, IDataValue<TValue>;
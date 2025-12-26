namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate void DataLocationAction<TValue>(DataLocation<TValue> location)
    where TValue : unmanaged, IDataValue<TValue>;
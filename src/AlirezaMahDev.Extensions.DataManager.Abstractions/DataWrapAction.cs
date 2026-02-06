namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate void DataWrapAction<TValue>(DataWrap<TValue> location)
    where TValue : unmanaged, IDataValue<TValue>;
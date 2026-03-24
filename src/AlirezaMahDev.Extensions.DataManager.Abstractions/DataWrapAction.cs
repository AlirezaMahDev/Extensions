namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate void DataWrapAction<TValue>(ref readonly DataWrap<TValue> location)
    where TValue : unmanaged, IDataValue<TValue>;
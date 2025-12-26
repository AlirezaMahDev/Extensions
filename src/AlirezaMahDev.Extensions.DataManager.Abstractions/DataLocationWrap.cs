namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public record struct DataLocationWrap<TValue, TWrap>(DataLocation<TValue> Location, TWrap Wrap)
    where TValue : unmanaged, IDataValue<TValue>;
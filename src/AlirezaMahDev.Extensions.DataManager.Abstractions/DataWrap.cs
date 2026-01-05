namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public record struct DataWrap(IDataAccess Access, DataLocation Location);

public record struct DataWrap<TValue>(IDataAccess Access, DataLocation<TValue> Location)
    where TValue : unmanaged, IDataValue<TValue>;

public record struct DataWrap<TValue, TWrap>(IDataAccess Access, DataLocation<TValue> Location, TWrap Wrap)
    where TValue : unmanaged, IDataValue<TValue>;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct DataWrap(IDataAccess Access, DataLocation Location);

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct DataWrap<TValue>(IDataAccess Access, DataLocation<TValue> Location)
    where TValue : unmanaged, IDataValue<TValue>;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct DataWrap<TValue, TWrap>(IDataAccess Access, DataLocation<TValue> Location, TWrap Wrap)
    where TValue : unmanaged, IDataValue<TValue>;
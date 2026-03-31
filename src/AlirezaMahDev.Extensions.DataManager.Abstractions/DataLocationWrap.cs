namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationWrap;

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct DataLocationWrap<TValue, TWrap>(IDataAccess access, DataLocation<TValue> location, TWrap wrap)
    where TValue : unmanaged, IDataValue<TValue>
{
    public readonly IDataAccess Access = access;
    public readonly DataLocation<TValue> Location = location;
    public readonly TWrap Wrap = wrap;
}
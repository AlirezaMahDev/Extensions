namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataWrap;

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct DataWrap<TValue, TWrap>(DataLocation<TValue> location, TWrap wrap)
    where TValue : unmanaged, IDataValue<TValue>
{
    public readonly DataLocation<TValue> Location = location;
    public readonly TWrap Wrap = wrap;
}
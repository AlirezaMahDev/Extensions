namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct DataWrap(IDataAccess access, DataLocation location)
{
    private readonly DataLocation _location = location;

    public IDataAccess Access
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = access;

    public ref readonly DataLocation Location
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this)._location;
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct DataWrap<TValue>(IDataAccess access, DataLocation<TValue> location)
    where TValue : unmanaged, IDataValue<TValue>
{
    private readonly IDataAccess _access = access;
    private readonly DataLocation<TValue> _location = location;

    public IDataAccess Access
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = access;

    public ref readonly DataLocation<TValue> Location
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this)._location;
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct DataWrap<TValue, TWrap>(IDataAccess access, DataLocation<TValue> location, TWrap wrap)
    where TValue : unmanaged, IDataValue<TValue>
{
    private readonly IDataAccess _access = access;
    private readonly DataLocation<TValue> _location = location;
    private readonly TWrap _wrap = wrap;

    public IDataAccess Access
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = access;

    public ref readonly DataLocation<TValue> Location
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this)._location;
        }
    }

    public ref readonly TWrap Wrap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this)._wrap;
        }
    }
}
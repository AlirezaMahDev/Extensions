namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct DataLocation(DataOffset offset) : IDataLocation<DataLocation>
{
    private readonly DataOffset _offset = offset;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Create(IDataAccess access, int length, out DataLocation result)
    {
        result = new(access.AllocateOffset(length));
    }


    public ref readonly DataOffset Offset
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this)._offset;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(in DataLocation other)
    {
        return _offset == other._offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(DataLocation other)
    {
        return Equals(in other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Memory<byte> GetMemory(IDataAccess access)
    {
        return access.ReadMemory(in Offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ref byte GetRef(IDataAccess access)
        => ref access.ReadRef(in Offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return XxHash3.Combine(in _offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is DataLocation dataLocation && Equals(in dataLocation);
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct DataLocation<TValue>(DataOffset offset) : IDataLocation<DataLocation<TValue>, TValue>
    where TValue : unmanaged, IDataValue<TValue>
{
    private readonly DataOffset _offset = offset;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Create(IDataAccess access, TValue @default, out DataLocation<TValue> result)
    {
        result = new(access.AllocateOffset(TValue.ValueSize));
        result.GetRefValue(access) = @default;
    }

    public ref readonly DataOffset Offset
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this)._offset;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(in DataLocation<TValue> other)
    {
        return _offset == other._offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(DataLocation<TValue> other)
    {
        return Equals(in other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Memory<byte> GetMemory(IDataAccess access)
    {
        return access.ReadMemory(in Offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ref byte GetRef(IDataAccess access)
        => ref access.ReadRef(in Offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return XxHash3.Combine(in _offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is DataLocation<TValue> dataLocation && Equals(in dataLocation);
    }
}
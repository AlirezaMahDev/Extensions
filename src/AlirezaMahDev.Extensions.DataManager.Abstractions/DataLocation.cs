namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct DataLocation<TValue>(DataOffset offset, IDataAccess access, IDataAlive alive)
    : IScopedRefReadOnlyEquatable<DataLocation<TValue>>
    where TValue : unmanaged, IDataValue<TValue>
{
    public readonly DataOffset Offset = offset;

    public IDataAccess Access
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = access;

    public IDataAlive Alive
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = alive;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Create(IDataAccess access, TValue @default, out DataLocation<TValue> result)
    {
        var alive = access.AllocationWithAlive(TValue.ValueSize, out var offset);
        result = new(offset, access, alive);
        result.UnsafeAccessRef((scoped ref value) => value = @default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Read(IDataAccess access, DataOffset offset, out DataLocation<TValue> result)
    {
        if (offset.IsDefault)
            Debugger.Break();
        result = new(offset, access, access.GetAlive(offset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return XxHash3.Combine(in Offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(ref readonly DataLocation<TValue> other)
    {
        return Offset == other.Offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(DataLocation<TValue> other)
    {
        return Equals(ref other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is DataLocation<TValue> dataLocation && Equals(ref dataLocation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(DataLocation<TValue> left, DataLocation<TValue> right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(DataLocation<TValue> left, DataLocation<TValue> right)
    {
        return !(left == right);
    }
}
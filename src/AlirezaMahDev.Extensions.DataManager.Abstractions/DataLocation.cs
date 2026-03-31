namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct DataLocation<TValue>(DataOffset offset, IDataMapFilePartOwner owner)
    : IScopedRefReadOnlyEquatable<DataLocation<TValue>>
    where TValue : unmanaged, IDataValue<TValue>
{
    public readonly DataOffset Offset = offset;
    public readonly IDataMapFilePartOwner Owner = owner;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Create(IDataAccess access, TValue @default, out DataLocation<TValue> result)
    {
        var offset = access.AllocateOffset(TValue.ValueSize);
        result = new(offset, access.GetOwner(ref offset)) { UnsafeRefValue = @default };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Read(IDataAccess access, DataOffset offset, out DataLocation<TValue> result)
    {
        result = new(offset, access.GetOwner(ref offset));
    }

    public ref byte UnsafeRef
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Owner.GetRef(Offset.Offset);
    }

    public ref TValue UnsafeRefValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.As<byte, TValue>(ref UnsafeRef);
    }

    public ref readonly TValue UnsafeRefReadOnlyValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.As<byte, TValue>(ref UnsafeRef);
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
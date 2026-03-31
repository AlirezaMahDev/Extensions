namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Auto)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct CellWrap<TValue, TData, TLink>(INerve<TData, TLink> nerve, DataLocation<TValue> location)
    : IScopedRefReadOnlyEquatable<CellWrap<TValue, TData, TLink>>
    where TValue : unmanaged, ICellValue<TValue>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public readonly DataLocation<TValue> Location = location;

    public DataLocationWrap<TValue, DataEmptyWrap> LocationWrap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return new(Nerve.Access, Location, DataEmptyWrap.Default);
        }
    }

    public INerve<TData, TLink> Nerve { get; } = nerve;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(scoped ref readonly CellWrap<TValue, TData, TLink> other)
    {
        return Location.Offset == other.Location.Offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(CellWrap<TValue, TData, TLink> other)
    {
        return Equals(ref other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is CellWrap<TValue, TData, TLink> cellWrap && Equals(cellWrap);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return Location.Offset.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Location}=>{Location.UnsafeRefValue}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(CellWrap<TValue, TData, TLink> left,
        CellWrap<TValue, TData, TLink> right)
    {
        return left.Equals(ref right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(CellWrap<TValue, TData, TLink> left,
        CellWrap<TValue, TData, TLink> right)
    {
        return !(left == right);
    }
}
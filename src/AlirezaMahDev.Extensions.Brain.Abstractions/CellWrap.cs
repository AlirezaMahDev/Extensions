namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct CellWrap<TCell, TValue, TData, TLink>(INerve<TData, TLink> nerve, in TCell cell)
    : ICellWrap<TCell, TValue, TData, TLink>, IInEquatable<CellWrap<TCell, TValue, TData, TLink>>
    where TCell : ICell
    where TValue : unmanaged, ICellValue<TValue>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    private readonly TCell _cell = cell;
    private readonly DataWrap<TValue> _location = new(nerve.Access, new(cell.Offset));

    public ref readonly TCell RefCell
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._cell;
    }

    public INerve<TData, TLink> Nerve { get; } = nerve;

    public ref readonly DataWrap<TValue> Location
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.AsRef(in this)._location;
        }
    }

    public ref readonly TValue RefValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Location.Location.GetRefValue(Nerve.Access);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(in CellWrap<TCell, TValue, TData, TLink> other)
    {
        return _cell.Offset == other._cell.Offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(CellWrap<TCell, TValue, TData, TLink> other)
    {
        return Equals(in other);
    }

    public override string ToString()
    {
        return $"{_cell} {RefValue}";
    }
}
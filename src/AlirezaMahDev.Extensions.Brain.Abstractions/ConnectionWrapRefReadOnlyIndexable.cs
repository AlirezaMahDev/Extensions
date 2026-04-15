namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public sealed class ConnectionWrapRefReadOnlyIndexable<TData, TLink> : IRefReadOnlyIndexable<
        ConnectionWrapRefReadOnlyIndexable<TData, TLink>, CellWrap<ConnectionValue<TLink>, TData, TLink>>,
    IDisposable
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    private readonly INerve<TData, TLink> _nerve;
    private readonly NativeRefList<DataOffset> _list;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConnectionWrapRefReadOnlyIndexable(CellWrap<ConnectionValue<TLink>, TData, TLink> cellWrap)
    {
        _nerve = cellWrap.Nerve;
        int count = cellWrap.Location.ReadLock((scoped ref readonly x) => x.Count);
        if (count == 0)
        {
            return;
        }

        _list = NativeRefList<DataOffset>.Create(count);
        foreach (var item in cellWrap.GetConnectionsWrap())
        {
            _list.Add(item.Location.Offset);
        }
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => _list.Length;
    }

    public ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            DataLocation<ConnectionValue<TLink>>.Read(_nerve.Access, _list[index], out var location);
            CellWrap<ConnectionValue<TLink>, TData, TLink> value = new(_nerve, location);
            return ref new ReadOnlyMemoryValue<CellWrap<ConnectionValue<TLink>, TData, TLink>>(value).Value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefReadOnlyIndexableEnumerator<ConnectionWrapRefReadOnlyIndexable<TData, TLink>,
        CellWrap<ConnectionValue<TLink>, TData, TLink>> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _list.Dispose();
    }
}
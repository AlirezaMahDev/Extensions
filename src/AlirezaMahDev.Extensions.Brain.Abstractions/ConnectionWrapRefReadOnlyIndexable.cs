namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public sealed class ConnectionWrapRefReadOnlyIndexable<TData, TLink> : IRefReadOnlyIndexable<
        ConnectionWrapRefReadOnlyIndexable<TData, TLink>, DataOffset>,
    IDisposable
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public readonly INerve<TData, TLink> Nerve;
    private readonly NativeRefList<DataOffset> _list;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConnectionWrapRefReadOnlyIndexable(CellWrap<ConnectionValue<TLink>, TData, TLink> cellWrap)
    {
        Nerve = cellWrap.Nerve;
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

    public ref readonly DataOffset this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _list[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public CellWrap<ConnectionValue<TLink>, TData, TLink> GetCellWrap(int index)
    {
        DataLocation<ConnectionValue<TLink>>.Read(Nerve.Access, _list[index], out var location);
        return new(Nerve, location);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public MemoryList<CellWrap<ConnectionValue<TLink>, TData, TLink>> GetCellWraps(int start, int end)
    {
        MemoryList<CellWrap<ConnectionValue<TLink>, TData, TLink>> list = new(end - start);
        for (int i = start; i < end; i++)
        {
            list.Add(GetCellWrap(i));
        }
        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefReadOnlyIndexableEnumerator<ConnectionWrapRefReadOnlyIndexable<TData, TLink>, DataOffset> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _list.Dispose();
    }
}
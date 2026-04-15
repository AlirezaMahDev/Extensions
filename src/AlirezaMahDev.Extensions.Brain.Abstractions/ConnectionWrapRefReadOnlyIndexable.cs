namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public sealed class ConnectionWrapRefReadOnlyIndexable<TData, TLink>(CellWrap<ConnectionValue<TLink>, TData, TLink> cellWrap) : IRefReadOnlyIndexable<
        ConnectionWrapRefReadOnlyIndexable<TData, TLink>, CellWrap<ConnectionValue<TLink>, TData, TLink>>,
    IDisposable
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    private readonly MemoryList<CellWrap<ConnectionValue<TLink>, TData, TLink>> _memoryList = cellWrap.GetConnectionsWrapMemory();

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => _memoryList.Count;
    }

    public Memory<CellWrap<ConnectionValue<TLink>, TData, TLink>> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => _memoryList.Memory;
    }

    public ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _memoryList.Memory.Span[index];
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
        _memoryList.Dispose();
    }
}
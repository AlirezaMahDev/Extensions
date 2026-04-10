namespace AlirezaMahDev.Extensions.Brain.Abstractions;

// ReSharper disable once RedundantExtendsListEntry
public readonly struct ConnectionWrapRefReadOnlyBlock<TData, TLink>(CellWrap<ConnectionValue<TLink>, TData, TLink> cellWrap)
    : IRefReadOnlyIndexable<ConnectionWrapRefReadOnlyBlock<TData, TLink>, CellWrap<ConnectionValue<TLink>, TData, TLink>>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> this[int index] =>
        throw new NotImplementedException();

    public int Length => throw new NotImplementedException();

    public ConnectionWrapRefReadOnlyBlockRefReadOnlyEnumerator<TData, TLink> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public ConnectionWrapRefReadOnlyBlock<TData, TLink> Slice(int start, int length)
    {
        throw new NotImplementedException();
    }
}

public readonly ref struct ConnectionWrapRefReadOnlyBlockRefReadOnlyEnumerator<TData, TLink>
    : IRefReadOnlyEnumerator<ConnectionWrapRefReadOnlyBlockRefReadOnlyEnumerator<TData, TLink>, CellWrap<ConnectionValue<TLink>, TData, TLink>>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> Current => throw new NotImplementedException();

    public bool MoveNext()
    {
        throw new NotImplementedException();
    }
}
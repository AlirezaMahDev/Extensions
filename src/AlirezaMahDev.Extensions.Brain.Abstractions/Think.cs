using System.Collections;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public record Think<TData, TLink>(
    TData Data,
    TLink Link,
    CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> Connection,
    Think<TData, TLink>? Previous)
    : IEnumerable<Think<TData, TLink>>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public Guid Id { get; } = Guid.CreateVersion7();

    public int Count { get; private init; }

    public TLink DifferenceLink { get; private init; }
    public TData DifferenceData { get; private init; }
    public ulong DifferenceWeight { get; private init; }
    public TLink AllDifferenceLink { get; private init; }
    public TData AllDifferenceData { get; private init; }
    public ulong AllDifferenceWeight { get; private init; }

    public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetNextConnectionWrap(int depth) =>
        Connection.GetConnectionsWrap().ToArray().AsMemory().Near(Link, depth);

    public Think<TData, TLink> Append(TData data,
        TLink link,
        CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connection)
    {
        var differenceData = NerveHelper.Difference(data, connection.NeuronWrap.RefData);
        var differenceLink = NerveHelper.Difference(link, connection.RefLink);
        var differenceWeight = connection.RefValue.Weight;
        return new(data, link, connection, this)
        {
            Count = Count + 1,
            DifferenceData = differenceData,
            DifferenceLink = differenceLink,
            DifferenceWeight = differenceWeight,
            AllDifferenceData = AllDifferenceData + differenceData,
            AllDifferenceLink = AllDifferenceLink + differenceLink,
            AllDifferenceWeight = AllDifferenceWeight + differenceWeight
        };
    }

    public IEnumerator<Think<TData, TLink>> GetEnumerator()
    {
        Stack<Think<TData, TLink>> stack = [];
        var current = this;
        while (current is not null)
        {
            stack.Push(current);
            current = current.Previous;
        }

        return stack.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public virtual bool Equals(Think<TData, TLink>? other)
    {
        return Id == other?.Id;
    }

    public override string ToString()
    {
        return $"Count:{Count} AllDifferenceData:{AllDifferenceData} AllDifferenceLink:{AllDifferenceLink
        } DifferenceData:{DifferenceData} DifferenceLink:{DifferenceLink}";
    }
}
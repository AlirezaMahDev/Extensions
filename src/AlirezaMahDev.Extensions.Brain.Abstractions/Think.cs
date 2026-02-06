using System.Collections;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public sealed class Think<TData, TLink> : IEnumerable<Think<TData, TLink>>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    private readonly Think<TData, TLink>? _previous;
    private readonly CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> _connectionWrap;

    public Think(
        CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap, Think<TData, TLink>? previous)
    {
        _connectionWrap = connectionWrap;
        _previous = previous;
        ConnectionWrap = connectionWrap;
    }

    public Guid Id { get; } = Guid.CreateVersion7();
    public int Count { get; private init; }

    public CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> ConnectionWrap { get; }

    public TLink DifferenceLink { get; private init; }
    public TData DifferenceData { get; private init; }
    public ulong DifferenceWeight { get; private init; }
    public TLink AllDifferenceLink { get; private init; }
    public TData AllDifferenceData { get; private init; }
    public ulong AllDifferenceWeight { get; private init; }

    public Memory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> GetNextConnectionWrap(TLink link, int depth)
    {
        var cellMemory = _connectionWrap.GetConnectionsWrapCache();
        return cellMemory.Memory.NearConnection(link, depth);
    }

    public Think<TData, TLink> Append(TData data,
        TLink link,
        CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connection)
    {
        var differenceData = NerveHelper.Difference(data, connection.NeuronWrap.RefData);
        var differenceLink = NerveHelper.Difference(link, connection.RefLink);
        var differenceWeight = connection.RefValue.Weight;
        return new(connection, this)
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
            current = current._previous;
        }

        return stack.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Think<TData, TLink>? other)
    {
        return Id == other?.Id;
    }

    public override string ToString()
    {
        return $"Count:{Count} AllDifferenceData:{AllDifferenceData} AllDifferenceLink:{AllDifferenceLink} DifferenceData:{DifferenceData} DifferenceLink:{DifferenceLink}";
    }
}
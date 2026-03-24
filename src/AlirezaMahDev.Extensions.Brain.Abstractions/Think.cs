namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public sealed class Think<TData, TLink> : IEnumerable<Think<TData, TLink>>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public Guid Id { get; }
    public int Count { get; }

    public ReadOnlyMemoryValue<TData> Data { get; }
    public ReadOnlyMemoryValue<TLink> Link { get; }
    public CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> ConnectionWrap { get; }
    private Think<TData, TLink>? Previous { get; }

    public TData DataDifferenceSum { get; }
    public TData DataDifferenceSumAbs { get; }
    public TLink LinkDifference { get; }

    public double ScoreSum { get; }
    public ulong WeightSum { get; }

    private Think(
        CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
    {
        Id = Guid.CreateVersion7();
        Count = 1;

        ConnectionWrap = connectionWrap;
        Previous = null;

        ScoreSum = connectionWrap.RefValue.Score;
        WeightSum = connectionWrap.RefValue.Weight;
    }

    private Think(
        ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap,
        Think<TData, TLink> previous)
    {
        Id = Guid.CreateVersion7();
        Count = previous.Count + 1;

        Data = data;
        Link = link;

        ConnectionWrap = connectionWrap;
        Previous = previous;

        var dataDifference = data.Value.Normalize() - connectionWrap.NeuronWrap.RefData.Normalize();
        DataDifferenceSum = previous.DataDifferenceSum + dataDifference;
        var dataDifferenceAbs = dataDifference.Abs();
        DataDifferenceSumAbs = previous.DataDifferenceSumAbs + dataDifferenceAbs;

        if (Count > 2)
        {
            LinkDifference = link.Value.Normalize() - connectionWrap.RefLink.Normalize();
        }

        ScoreSum = previous.ScoreSum + connectionWrap.RefValue.Score;
        WeightSum = previous.WeightSum + connectionWrap.RefValue.Weight;
    }

    public static Think<TData, TLink> Create(CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connection)
    {
        return new(connection);
    }

    public Think<TData, TLink> Append(ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connection)
    {
        return new(data, link, connection, this);
    }

    [MustDisposeResource]
    public IReadonlyMemoryList<ReadOnlyMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>
        GetNextConnectionWrap(
            PredictValueRef<TLink> link,
            int depth)
    {
        return ConnectionWrap.GetConnectionsWrapCache().Memory.NearConnection(link, depth);
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

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

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
        return $"Count:{Count}" +
               " " +
               $"DataDifferenceSum:{DataDifferenceSum}" +
               " " +
               $"LinkDifference:{LinkDifference}";
    }
}
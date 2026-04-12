namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public sealed class Think<TData, TLink> : IEnumerable<Think<TData, TLink>>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public readonly int Count;

    public readonly ReadOnlyMemoryValue<TData> Data;
    public readonly ReadOnlyMemoryValue<TLink> Link;
    public readonly CellWrap<ConnectionValue<TLink>, TData, TLink> ConnectionWrap;
    public readonly Think<TData, TLink>? Previous;

    public TData DataDifference;
    public TLink LinkDifference;

    public readonly double ScoreSum;
    public readonly ulong WeightSum;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Think(
        CellWrap<ConnectionValue<TLink>, TData, TLink> connectionWrap)
    {
        Count = 1;

        ConnectionWrap = connectionWrap;
        Previous = null;

        ScoreSum = connectionWrap.Location.ReadLock((scoped ref readonly x) => x).Score;
        WeightSum = connectionWrap.Location.ReadLock((scoped ref readonly x) => x).Weight;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Think(
        ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        CellWrap<ConnectionValue<TLink>, TData, TLink> connectionWrap,
        Think<TData, TLink> previous)
    {
        Count = previous.Count + 1;

        Data = data;
        Link = link;

        ConnectionWrap = connectionWrap;
        Previous = previous;

        using var connectionWrapNeuronWrap = connectionWrap.NeuronWrap.Location.ReadLock();
        using var connectionValue = connectionWrap.Location.ReadLock();
        DataDifference = TData.ThinkDifference(ref previous.DataDifference,
            in data.Value,
            in connectionWrapNeuronWrap.RefReadOnlyValue.Data);
        if (Count > 2)
        {
            LinkDifference = TLink.ThinkDifference(ref previous.LinkDifference, in link.Value, in connectionValue.RefReadOnlyValue.Link);
        }

        ScoreSum = previous.ScoreSum + connectionValue.RefReadOnlyValue.Score;
        WeightSum = previous.WeightSum + connectionValue.RefReadOnlyValue.Weight;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Think<TData, TLink> Create(CellWrap<ConnectionValue<TLink>, TData, TLink> connection)
    {
        return new(connection);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Think<TData, TLink> Append(ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        CellWrap<ConnectionValue<TLink>, TData, TLink> connection)
    {
        return new(data, link, connection, this);
    }

    [MustDisposeResource]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public MemoryList<Memory<CellWrap<ConnectionValue<TLink>, TData, TLink>>>? GetNextConnectionWrap(
        PredictValueRef<TLink> link,
        int depth)
    {
        using var connectionsWrapRefReadOnlyIndexable = ConnectionWrap.GetConnectionsWrapMemory();
        return connectionsWrapRefReadOnlyIndexable?.NearConnection(ref link, depth);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return $"Count:{Count}" +
               " " +
               $"DataDifference:{DataDifference}" +
               " " +
               $"LinkDifference:{LinkDifference}";
    }
}
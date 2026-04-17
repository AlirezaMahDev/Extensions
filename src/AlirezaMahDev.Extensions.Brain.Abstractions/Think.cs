namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Auto)]
public readonly struct Think<TData, TLink>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public readonly int Count;

    public readonly ReadOnlyMemoryValue<TData> Data;
    public readonly ReadOnlyMemoryValue<TLink> Link;
    public readonly CellWrap<ConnectionValue<TLink>, TData, TLink> ConnectionWrap;

    public readonly TData DataDifference;
    public readonly TLink LinkDifference;

    public readonly double ScoreSum;
    public readonly ulong WeightSum;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Think(in CellWrap<ConnectionValue<TLink>, TData, TLink> connectionWrap)
    {
        Count = 1;

        ConnectionWrap = connectionWrap;

        ScoreSum = connectionWrap.Location.ReadLock((scoped ref readonly x) => x).Score;
        WeightSum = connectionWrap.Location.ReadLock((scoped ref readonly x) => x).Weight;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Think(
        in ReadOnlyMemoryValue<TData> data,
        in ReadOnlyMemoryValue<TLink> link,
        in CellWrap<ConnectionValue<TLink>, TData, TLink> connectionWrap,
        in Think<TData, TLink> previous)
    {
        Count = previous.Count + 1;

        Data = data;
        Link = link;

        ConnectionWrap = connectionWrap;

        using var connectionWrapNeuronWrap = connectionWrap.NeuronWrap.Location.ReadLock();
        using var connectionValue = connectionWrap.Location.ReadLock();
        DataDifference = TData.ThinkDifference(in previous.DataDifference,
            in data.Value,
            in connectionWrapNeuronWrap.RefReadOnlyValue.Data);
        if (Count > 2)
        {
            LinkDifference = TLink.ThinkDifference(in previous.LinkDifference,
                in link.Value,
                in connectionValue.RefReadOnlyValue.Link);
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
    public Think<TData, TLink> Append(in ReadOnlyMemoryValue<TData> data,
        in ReadOnlyMemoryValue<TLink> link,
        in CellWrap<ConnectionValue<TLink>, TData, TLink> connection)
    {
        return new(in data, in link, in connection, in this);
    }

    [MustDisposeResource]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public NativeRefList<Range> GetNextConnectionWrap(
        PredictValue<TLink> link,
        int depth)
    {
        var connectionsWrapRefReadOnlyIndexable = ConnectionWrap.GetConnectionWrapRefReadOnlyIndexable();
        return connectionsWrapRefReadOnlyIndexable.NearConnection(ref link, depth);
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
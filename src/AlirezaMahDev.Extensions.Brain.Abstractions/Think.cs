using System.Collections;

using AlirezaMahDev.Extensions.Abstractions;

using JetBrains.Annotations;

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

    public TData AllDifferenceData { get; }
    public TLink AllDifferenceLink { get; }

    public TData AvgDifferenceData { get; }
    public TLink AvgDifferenceLink { get; }

    public double AllScore { get; }
    public ulong AllWeight { get; }

    private Think(
           ReadOnlyMemoryValue<TData> data,
           ReadOnlyMemoryValue<TLink> link,
           CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
    {
        Id = Guid.CreateVersion7();
        Count = 1;

        Data = data;
        Link = link;

        ConnectionWrap = connectionWrap;
        Previous = null;

        AllDifferenceData = NerveHelper.Difference(
            TData.Normalize(in data.Value),
            TData.Normalize(in connectionWrap.NeuronWrap.RefData)
        );
        AllDifferenceLink = NerveHelper.Difference(
            TLink.Normalize(in link.Value),
            TLink.Normalize(in connectionWrap.RefLink)
        );

        AvgDifferenceData = AllDifferenceData / Count;
        AvgDifferenceLink = AllDifferenceLink / Count;

        AllScore = connectionWrap.RefValue.Score;
        AllWeight = connectionWrap.RefValue.Weight;
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

        AllDifferenceData = previous.AllDifferenceData + NerveHelper.Difference(
            TData.Normalize(in data.Value),
            TData.Normalize(in connectionWrap.NeuronWrap.RefData)
        );
        AllDifferenceLink = previous.AllDifferenceLink + NerveHelper.Difference(
            TLink.Normalize(in link.Value),
            TLink.Normalize(in connectionWrap.RefLink)
        );

        AvgDifferenceData = AllDifferenceData / Count;
        AvgDifferenceLink = AllDifferenceLink / Count;

        AllScore = previous.AllScore + connectionWrap.RefValue.Score;
        AllWeight = previous.AllWeight + connectionWrap.RefValue.Weight;
    }

    public static Think<TData, TLink> Create(ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connection) =>
            new(data, link, connection);

    public Think<TData, TLink> Append(ReadOnlyMemoryValue<TData> data,
        ReadOnlyMemoryValue<TLink> link,
        CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connection) =>
            new(data, link, connection, this);

    [MustDisposeResource]
    public IReadonlyMemoryList<ReadOnlyMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>
        GetNextConnectionWrap(
            PredictValueRef<TLink> link,
            int depth) =>
        ConnectionWrap.GetConnectionsWrapCache().Memory.NearConnection(link, depth);

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

    public bool Equals(Think<TData, TLink>? other)
    {
        return Id == other?.Id;
    }

    public override string ToString()
    {
        return $"Count:{Count}" + " " +
        $"AllDifferenceData:{AllDifferenceData}" + " " +
        $"AllDifferenceLink:{AllDifferenceLink}" + " " +
        $"AvgDifferenceData:{AvgDifferenceData}" + " " +
        $"AvgDifferenceLink:{AvgDifferenceLink}";
    }
}
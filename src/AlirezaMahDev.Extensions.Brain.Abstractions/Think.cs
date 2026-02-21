using System.Collections;

using AlirezaMahDev.Extensions.Abstractions;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public sealed class Think<TData, TLink>(
    ReadOnlyMemoryValue<TData> data,
    ReadOnlyMemoryValue<TLink> link,
    CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap,
    Think<TData, TLink>? previous)
    : IEnumerable<Think<TData, TLink>>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    private readonly Think<TData, TLink>? _previous = previous;
    private readonly CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> _connectionWrap = connectionWrap;

    public Guid Id { get; } = Guid.CreateVersion7();
    public int Count { get; private init; }

    public ReadOnlyMemoryValue<TData> Data { get; } = data;
    public ReadOnlyMemoryValue<TLink> Link { get; } = link;
    public CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> ConnectionWrap { get; } = connectionWrap;

    public TLink DifferenceLink { get; private init; }
    public TData DifferenceData { get; private init; }

    public double Score { get; private init; }
    public ulong Weight { get; private init; }

    public TLink AllDifferenceLink { get; private init; }
    public TData AllDifferenceData { get; private init; }
    public double AllScore { get; private init; }
    public ulong AllWeight { get; private init; }

    [MustDisposeResource]
    public IReadonlyMemoryList<ReadOnlyMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>
        GetNextConnectionWrap(
            PredictValueRef<TLink> link,
            int depth) =>
        _connectionWrap.GetConnectionsWrapCache().Memory.NearConnection(link, depth);

    public Think<TData, TLink> Append(ReadOnlySpanValue<TData> data,
        ReadOnlySpanValue<TLink> link,
        CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connection)
    {
        var differenceData = NerveHelper.Difference(
            TData.Normalize(in data.Value),
            TData.Normalize(in connection.NeuronWrap.RefData)
        );
        var differenceLink = NerveHelper.Difference(
            TLink.Normalize(in link.Value),
            TLink.Normalize(in connection.RefLink)
        );

        var weight = connection.RefValue.Weight;
        var score = connection.RefValue.Score;
        return new(data.Value, link.Value, connection, this)
        {
            Count = Count + 1,
            DifferenceData = differenceData,
            DifferenceLink = differenceLink,
            Score = score,
            Weight = weight,
            AllDifferenceData = AllDifferenceData + differenceData,
            AllDifferenceLink = AllDifferenceLink + differenceLink,
            AllScore = AllScore + score,
            AllWeight = AllWeight + weight
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
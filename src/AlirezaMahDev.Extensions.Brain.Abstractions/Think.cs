using System.Collections;
using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public record Think<TData, TLink>(
    TData Data,
    TLink Link,
    IConnection<TData, TLink> Connection,
    Think<TData, TLink>? Previous)
    : IComparable<Think<TData, TLink>>,
        IEnumerable<Think<TData, TLink>>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public int Count { get; private init; }

    public TLink DifferenceLink { get; private init; }
    public TData DifferenceData { get; private init; }

    public TLink AllDifferenceLink { get; private init; }
    public TData AllDifferenceData { get; private init; }


    public Think<TData, TLink> Append(TData data, TLink link, IConnection<TData, TLink> connection)
    {
        var differenceData = data - connection.GetNeuron().RefData;
        if (differenceData.CompareTo(default) < 0)
        {
            differenceData = connection.GetNeuron().RefData - data;
        }

        var differenceLink = link - connection.RefLink;
        if (differenceLink.CompareTo(default) < 0)
        {
            differenceLink = connection.RefLink - link;
        }

        return new(data, link, connection, this)
        {
            Count = Count + 1,
            DifferenceData = differenceData,
            DifferenceLink = differenceLink,
            AllDifferenceData = AllDifferenceData + differenceData,
            AllDifferenceLink = AllDifferenceLink + differenceLink
        };
    }


    public async ValueTask<Think<TData, TLink>> AppendAsync(TData data, TLink link, IConnection<TData, TLink> connection, CancellationToken cancellationToken = default)
    {
        var differenceData = data - (await connection.GetNeuronAsync(cancellationToken)).RefData;
        if (differenceData.CompareTo(default) < 0)
        {
            differenceData = (await connection.GetNeuronAsync(cancellationToken)).RefData - data;
        }

        var differenceLink = link - connection.RefLink;
        if (differenceLink.CompareTo(default) < 0)
        {
            differenceLink = connection.RefLink - link;
        }

        return new(data, link, connection, this)
        {
            Count = Count + 1,
            DifferenceData = differenceData,
            DifferenceLink = differenceLink,
            AllDifferenceData = AllDifferenceData + differenceData,
            AllDifferenceLink = AllDifferenceLink + differenceLink
        };
    }

    public int CompareTo(Think<TData, TLink>? other)
    {
        if (other is null)
            return 1;

        var allDifferenceData = AllDifferenceData.CompareTo(other.AllDifferenceData);
        if (allDifferenceData != 0)
            return allDifferenceData;

        var alDifferenceLink = AllDifferenceLink.CompareTo(other.AllDifferenceLink);
        if (alDifferenceLink != 0)
            return alDifferenceLink;

        return 0;
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
}
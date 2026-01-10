using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public readonly record struct DataPairLink<TData, TLink>(TData Data, TLink Link)
    : IComparable<DataPairLink<TData, TLink>>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public int CompareTo(DataPairLink<TData, TLink> other)
    {
        var link = Link.CompareTo(other.Link);
        if (link != 0)
            return link;

        var data = Data.CompareTo(other.Data);
        if (data != 0)
            return data;

        return 0;
    }
}
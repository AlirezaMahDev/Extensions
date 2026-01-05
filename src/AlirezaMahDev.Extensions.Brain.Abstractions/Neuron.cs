using System.Numerics;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public readonly record struct Neuron<TData, TLink>(DataOffset Offset) : ICell
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public readonly DataOffset RefOffset = Offset;
    public DataOffset Offset => RefOffset;
}
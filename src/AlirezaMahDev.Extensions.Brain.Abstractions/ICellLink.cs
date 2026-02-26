using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellLink<TSelf> : IEquatable<TSelf>,
    IComparable<TSelf>,
    IAdditionOperators<TSelf, TSelf, TSelf>,
    ISubtractionOperators<TSelf, TSelf, TSelf>,
    IDivisionOperators<TSelf, int, TSelf>
    where TSelf : unmanaged, ICellLink<TSelf>
{
    static abstract TSelf Normalize(in TSelf self);
}
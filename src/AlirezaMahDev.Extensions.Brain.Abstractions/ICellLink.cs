using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellLink<TSelf> : IEquatable<TSelf>,
    IComparable<TSelf>,
    IAdditionOperators<TSelf, TSelf, TSelf>,
    ISubtractionOperators<TSelf, TSelf, TSelf>
    where TSelf : unmanaged, ICellLink<TSelf>;
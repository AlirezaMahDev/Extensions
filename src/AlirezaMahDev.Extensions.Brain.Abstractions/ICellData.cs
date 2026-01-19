using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellData<TSelf> : IEquatable<TSelf>,
    IComparable<TSelf>,
    IAdditionOperators<TSelf, TSelf, TSelf>,
    ISubtractionOperators<TSelf, TSelf, TSelf>
    where TSelf : unmanaged, ICellData<TSelf>;
using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellBase<TSelf> : IEquatable<TSelf>,
    IComparable<TSelf>,
    IMultiplyOperators<TSelf, TSelf, TSelf>,
    IDivisionOperators<TSelf, TSelf, TSelf>,
    IAdditionOperators<TSelf, TSelf, TSelf>,
    ISubtractionOperators<TSelf, TSelf, TSelf>
    where TSelf : unmanaged, ICellBase<TSelf>
{
    public TSelf Normalize();
    public TSelf UnNormalize();
    public TSelf Abs();
}

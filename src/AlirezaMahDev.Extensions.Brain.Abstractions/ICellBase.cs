namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellBase<TSelf> : IInEquatable<TSelf>,
    IInEqualityOperators<TSelf, TSelf, bool>,
    IInComparable<TSelf>,
    IMultiplyOperators<TSelf, TSelf, TSelf>,
    IDivisionOperators<TSelf, TSelf, TSelf>,
    IAdditionOperators<TSelf, TSelf, TSelf>,
    ISubtractionOperators<TSelf, TSelf, TSelf>
    where TSelf : unmanaged, ICellBase<TSelf>
{
    TSelf Normalize();
    TSelf UnNormalize();
    TSelf Abs();
}
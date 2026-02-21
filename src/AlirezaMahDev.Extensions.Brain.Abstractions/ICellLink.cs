using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellLink<TSelf> : IEquatable<TSelf>,
    IComparable<TSelf>,
    IAdditionOperators<TSelf, TSelf, TSelf>,
    ISubtractionOperators<TSelf, TSelf, TSelf>
    where TSelf : unmanaged, ICellLink<TSelf>
{
    static abstract TSelf Normalize(in TSelf self);
}

public static class CellLinkExtensions
{
    extension<T>(ref T value)
        where T : unmanaged, ICellLink<T>
    {
        public T Normalize() =>
            T.Normalize(in value);
    }
}
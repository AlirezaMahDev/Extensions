using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellData<TSelf> : IEquatable<TSelf>,
    IComparable<TSelf>,
    IAdditionOperators<TSelf, TSelf, TSelf>,
    ISubtractionOperators<TSelf, TSelf, TSelf>
    where TSelf : unmanaged, ICellData<TSelf>
{
    static abstract TSelf Normalize(in TSelf self);
}

public static class CellDataExtensions
{
    extension<T>(ref T value)
        where T : unmanaged, ICellData<T>
    {
        public T Normalize() =>
            T.Normalize(in value);
    }
}
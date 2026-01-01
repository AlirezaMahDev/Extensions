using System.Numerics;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CellWrapExtensions
{
    extension<TCellWrap, TValue, TCell, TData, TLink>(TCellWrap wrap)
        where TCellWrap : ICellWrap<TCell, TValue, TData, TLink>
        where TValue : unmanaged, IDataValue<TValue>
        where TCell : ICell
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
    }
}
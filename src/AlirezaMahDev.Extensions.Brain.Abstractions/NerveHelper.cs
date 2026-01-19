using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveHelper
{
    public static T Difference<T>(T a, T b)
        where T : IComparable<T>, ISubtractionOperators<T, T, T> =>
        a - b is { } da && da.CompareTo(default) >= 0 ? da : b - a;
}

public static class NerveHelper<TData, TLink>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public static Memory<Comparison<Think<TData, TLink>>> ThinkComparisons { get; } = new(
    [
        (x, y) => x.AllDifferenceData.CompareTo(y.AllDifferenceData),
        (x, y) => x.AllDifferenceLink.CompareTo(y.AllDifferenceLink)
    ]);


    public static Memory<Comparison<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>> GetConnectionComparisons(
        DataPairLink<TData, TLink> pair)
    {
        return new([
            CellWrap<Connection,ConnectionValue<TLink>,TData, TLink>.ComparerOnData(pair.Data),
            CellWrap<Connection,ConnectionValue<TLink>,TData, TLink>.ComparerOnLink(pair.Link),
            CellWrap<Connection,ConnectionValue<TLink>,TData, TLink>
                .ComparerOnWeight<CellWrap<Connection,ConnectionValue<TLink>,TData, TLink>, ConnectionValue<TLink>, Connection>()
        ]);
    }
}
using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;

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
    public static ComparisonChain<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> SleepComparisons { get; } =
        ComparisonChain<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>
            .ChainOrderBy(x => x.NeuronWrap.RefData)
            .ChainOrderBy(x => x.RefLink)
            .ChainOrderByDescending(x => x.RefValue.Weight)
            .UnWrap;

    public static ComparisonChain<Think<TData, TLink>> ThinkComparisons { get; } =
        ComparisonChain<Think<TData, TLink>>
            .ChainOrderBy(x => x.AllDifferenceData)
            .ChainOrderBy(x => x.AllDifferenceLink)
            .ChainOrderByDescending(x => x.AllDifferenceWeight)
            .UnWrap;

    public static ComparisonChain<DataPairLink<TData, TLink>> NearComparisons { get; } =
        ComparisonChain<DataPairLink<TData, TLink>>
            .ChainOrderBy(x => x.Data)
            .ChainOrderBy(x => x.Link)
            .UnWrap;

    public static ComparisonChain<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>> NextComparisons { get; } =
        ComparisonChain<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>
            .ChainOrderBy(x => x.RefLink)
            .ChainOrderByDescending(x => x.RefValue.Weight)
            .UnWrap;
    public static ComparisonChain<TLink> NearNextComparisons { get; } =
        ComparisonChain<TLink>
            .ChainOrderBy(x => x)
            .UnWrap;
}
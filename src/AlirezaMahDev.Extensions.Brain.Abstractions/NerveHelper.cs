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
    public static ComparisonChain<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>
        SleepComparisons { get; } =
        ComparisonChain<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>
            .ChainOrderBy(x => x.NeuronWrap.RefData)
            .ChainOrderBy(x => x.RefLink)
            .ChainOrderBy(x => x.RefValue.RefScore)
            .ChainOrderBy(x => x.RefValue.Weight)
            .UnWrap;

    public static ComparisonChain<Think<TData, TLink>> ThinkComparisons { get; } =
        ComparisonChain<Think<TData, TLink>>
            .ChainOrderBy(x => x.AllDifferenceData)
            .ChainOrderBy(x => x.AllDifferenceLink)
            .ChainOrderBy(x => x.AllDifferenceScore)
            .ChainOrderBy(x => x.AllDifferenceWeight)
            .UnWrap;

    public static ComparisonChain<ThinkValue<TData, TLink>> NearComparisons { get; } =
        ComparisonChain<ThinkValue<TData, TLink>>
            .ChainOrderBy(x => x.Data.Value)
            .ChainOrderBy(x => x.Link.Value)
            .ChainOrderBy(x => x.Score)
            .ChainOrderBy(x => x.Weight)
            .UnWrap;

    public static ComparisonChain<PredictValue<TLink>> NearNextComparisons { get; } =
        ComparisonChain<PredictValue<TLink>>
            .ChainOrderBy(x => x.Link.Value)
            .ChainOrderBy(x => x.Score)
            .ChainOrderBy(x => x.Weight)
            .UnWrap;
}
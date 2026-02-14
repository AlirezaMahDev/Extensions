using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveHelper
{
    public static T Difference<T>(in T a, in T b)
        where T : IComparable<T>, ISubtractionOperators<T, T, T> =>
        a - b is { } da && da.CompareTo(default) >= 0 ? da : b - a;
}

public static class NerveHelper<TData, TLink>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public static ComparisonChain<ThinkValueRef<TData, TLink>> SleepComparisons { get; } =
        ComparisonChain<ThinkValueRef<TData, TLink>>
            .ChainOrderBy(x => x.Data)
            .ChainOrderBy(x => x.Link)
            .ChainOrderBy(x => x.Score)
            .ChainOrderBy(x => x.Weight)
            .UnWrap;

    public static ComparisonChain<Think<TData, TLink>> ThinkComparisons { get; } =
        ComparisonChain<Think<TData, TLink>>
            .ChainOrderBy(x => x.AllDifferenceData)
            .ChainOrderBy(x => x.AllDifferenceLink)
            .ChainOrderByDescending(x => x.AllScore)
            .ChainOrderByDescending(x => x.AllWeight)
            .UnWrap;


    public static ComparisonChain<PredictValueRef<TLink>> NearNextComparisons { get; } =
        ComparisonChain<PredictValueRef<TLink>>
            .ChainOrderBy(x => x.Link)
            .ChainOrderByDescending(x => x.Score)
            .ChainOrderByDescending(x => x.Weight)
            .Merge()
            .UnWrap;
}
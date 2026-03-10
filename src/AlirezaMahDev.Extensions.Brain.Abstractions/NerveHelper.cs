using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveHelper<TData, TLink>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public static ComparisonChain<ThinkValueRef<TData, TLink>> SleepComparisons { get; } =
        ComparisonChain<ThinkValueRef<TData, TLink>>
            .ChainOrderBy(x => x.Data.Normalize())
            .ChainOrderBy(x => x.Link.Normalize())
            .ChainOrderBy(x => x.Score)
            .ChainOrderBy(x => x.Weight)
            .UnWrap;

    public static ComparisonChain<Think<TData, TLink>> ThinkComparisons { get; } =
        ComparisonChain<Think<TData, TLink>>
            .ChainOrderBy(x => x.LinkDifference.Abs())
            .ChainOrderBy(x => x.DataDifferenceSumAbs)
            .ChainOrderByDescending(x => x.ScoreSum)
            .ChainOrderByDescending(x => x.WeightSum)
            .UnWrap;

    public static ComparisonChain<PredictValueRef<TLink>> NearNextComparisons { get; } =
        ComparisonChain<PredictValueRef<TLink>>
            .ChainOrderBy(x => x.Link.Normalize())
            .ChainOrderByDescending(x => x.Score)
            .ChainOrderByDescending(x => x.Weight)
            .Merge()
            .UnWrap;
}
namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveHelper<TData, TLink>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public static ScopedComparisonChain<ThinkValueRef<TData, TLink>> SleepComparisons { get; }
    public static ScopedComparisonChain<Think<TData, TLink>> ThinkComparisons { get; }
    public static ScopedComparisonChain<PredictValueRef<TLink>> NearNextComparisons { get; }

    static NerveHelper()
    {
        SleepComparisons = SleepComparisonWrap()
            .ChainOrderBy(x => x.Score)
            .ChainOrderBy(x => x.Weight)
            .UnWrap;

        NearNextComparisons = NearNextComparisonWrap()
            .ChainOrderByDescending(x => x.Score)
            .ChainOrderByDescending(x => x.Weight)
            .Merge()
            .UnWrap;

        ThinkComparisons =
            ThinkComparisonWrap()
                .ChainOrderByDescending(x => x.ScoreSum)
                .ChainOrderByDescending(x => x.WeightSum)
                .UnWrap;
    }

    private static ComparisonWrap<ScopedComparisonChain<ThinkValueRef<TData, TLink>>, ThinkValueRef<TData, TLink>>
        SleepComparisonWrap()
    {
        using var dataSleepComparisonEnumerator =
            TData.SleepComparison().GetComparisonChains().GetEnumerator();
        dataSleepComparisonEnumerator.MoveNext();
        var currentDataSleepReadOnlyComparisonEnumerator =
            dataSleepComparisonEnumerator.Current.CurrentComparison;
        var comparisonWrap =
            ScopedComparisonChain<ThinkValueRef<TData, TLink>>.ChainOrder((scoped ref readonly a, scoped ref readonly b) =>
                currentDataSleepReadOnlyComparisonEnumerator(in a.Data, in b.Data));
        while (dataSleepComparisonEnumerator.MoveNext())
        {
            currentDataSleepReadOnlyComparisonEnumerator = dataSleepComparisonEnumerator.Current.CurrentComparison;
            comparisonWrap.ChainOrder((scoped ref readonly a, scoped ref readonly b) =>
                currentDataSleepReadOnlyComparisonEnumerator(in a.Data, in b.Data));
        }

        using var linkSleepComparisonEnumerator =
            TLink.SleepComparison().GetComparisonChains().GetEnumerator();
        while (linkSleepComparisonEnumerator.MoveNext())
        {
            var currenLinkSleepReadOnlyComparisonEnumerator =
                linkSleepComparisonEnumerator.Current.CurrentComparison;
            comparisonWrap.ChainOrder((scoped ref readonly a, scoped ref readonly b) =>
                currenLinkSleepReadOnlyComparisonEnumerator(in a.Link, in b.Link));
        }

        return comparisonWrap;
    }

    private static ComparisonWrap<ScopedComparisonChain<PredictValueRef<TLink>>, PredictValueRef<TLink>>
        NearNextComparisonWrap()
    {
        using var comparisonEnumerator =
            TLink.NextComparison().GetComparisonChains().GetEnumerator();
        comparisonEnumerator.MoveNext();
        var currentReadOnlyComparisonEnumerator =
            comparisonEnumerator.Current.CurrentComparison;
        var comparisonWrap =
            ScopedComparisonChain<PredictValueRef<TLink>>.ChainOrder((scoped ref readonly a, scoped ref readonly b) =>
                currentReadOnlyComparisonEnumerator(in a.Link, in b.Link));
        while (comparisonEnumerator.MoveNext())
        {
            currentReadOnlyComparisonEnumerator = comparisonEnumerator.Current.CurrentComparison;
            comparisonWrap.ChainOrder((scoped ref readonly a, scoped ref readonly b) =>
                currentReadOnlyComparisonEnumerator(in a.Link, in b.Link));
        }

        return comparisonWrap;
    }

    private static ComparisonWrap<ScopedComparisonChain<Think<TData, TLink>>, Think<TData, TLink>>
        ThinkComparisonWrap()
    {
        using var dataComparisonEnumerator =
            TData.ThinkComparison().GetComparisonChains().GetEnumerator();
        dataComparisonEnumerator.MoveNext();
        var currentDataReadOnlyComparisonEnumerator =
            dataComparisonEnumerator.Current.CurrentComparison;
        var comparisonWrap =
            ScopedComparisonChain<Think<TData, TLink>>.ChainOrder((scoped ref readonly a, scoped ref readonly b) =>
                currentDataReadOnlyComparisonEnumerator(ref a.DataDifference, ref b.DataDifference));
        while (dataComparisonEnumerator.MoveNext())
        {
            currentDataReadOnlyComparisonEnumerator = dataComparisonEnumerator.Current.CurrentComparison;
            comparisonWrap.ChainOrder((scoped ref readonly a, scoped ref readonly b) =>
                currentDataReadOnlyComparisonEnumerator(ref a.DataDifference, ref b.DataDifference));
        }

        foreach (var comparisonChain in TLink.ThinkComparison().GetComparisonChains())
        {
            comparisonWrap.ChainOrder((scoped ref readonly a, scoped ref readonly b) =>
                comparisonChain.CurrentComparison(ref a.LinkDifference, ref b.LinkDifference));
        }

        return comparisonWrap;
    }
}
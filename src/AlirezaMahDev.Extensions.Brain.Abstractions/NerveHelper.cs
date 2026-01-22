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
    public static
        ComparisonBuilder<ComparisonCollectionChain<ScoreSortItem<Think<TData, TLink>>>,
            ScoreSortItem<Think<TData, TLink>>> ThinkComparisons { get; } =
        comparisonChain => comparisonChain
            .WithBy(x => x.Value.AllDifferenceData)
            .WithBy(x => x.Value.AllDifferenceLink)
            .ThenBy(x => x.Value.AllDifferenceLink)
            .ThenByDescending(x => x.Value.AllDifferenceWeight);

    public static ComparisonBuilder<
            ComparisonCollectionChain<ScoreSortItem<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>,
            ScoreSortItem<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>
        ConnectionComparisons(DataPairLink<TData, TLink> pair) =>
        comparisonChain => comparisonChain
            .WithBy(x => NerveHelper.Difference(x.Value.NeuronWrap.RefData, pair.Data))
            .WithBy(x => NerveHelper.Difference(x.Value.RefLink, pair.Link))
            .ThenBy(x => NerveHelper.Difference(x.Value.RefLink, pair.Link))
            .ThenByDescending(x => x.Value.RefValue.Weight);

    public static ComparisonBuilder<
            ComparisonCollectionChain<ScoreSortItem<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>,
            ScoreSortItem<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>
        NextConnectionComparisons(TLink link) =>
        comparisonChain =>
            comparisonChain
                .WithBy(x => NerveHelper.Difference(x.Value.RefLink, link))
                .ThenByDescending(x => x.Value.RefValue.Weight);
}
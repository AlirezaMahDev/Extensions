namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparisonCollectionExtensions
{
    extension<T>(ComparisonCollection<T> unwrap)
    {
        public ComparisonWrap<ComparisonCollection<T>, T> Wrap() =>
            new(unwrap);
    }

    extension<TComparisonCollection, T>(ComparisonWrap<TComparisonCollection, T> wrap)
        where TComparisonCollection : struct, IComparisonCollection<T>
        where T : allows ref struct
    {
        public ComparisonWrap<TComparisonCollection, T> With(Comparison<T> comparison) =>
            new(wrap.UnWrap with { Enumerable = wrap.UnWrap.Enumerable.Append(comparison) });

        public ComparisonWrap<TComparisonCollection, T> WithDescending(Comparison<T> comparison) =>
            new(wrap.UnWrap with { Enumerable = wrap.UnWrap.Enumerable.Append((x, y) => comparison(y, x)) });

        public ComparisonWrap<TComparisonCollection, T> With(Comparison<T> comparison,
            ComparisonBuilder<ComparisonChain<T>, T> builder) =>
            wrap.With(builder(ComparisonChain<T>.ChainOrder(comparison)).UnWrap.Comparison);

        public ComparisonWrap<TComparisonCollection, T> WithDescending(Comparison<T> comparison,
            ComparisonBuilder<ComparisonChain<T>, T> builder) =>
            wrap.With(builder(ComparisonChain<T>.ChainOrderDescending(comparison)).UnWrap.Comparison);

        public ComparisonWrap<TComparisonCollection, T> WithBy<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey> =>
            wrap.With((x, y) => func(x).CompareTo(func(y)));

        public ComparisonWrap<TComparisonCollection, T> WithByDescending<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey> =>
            wrap.WithDescending((x, y) => func(x).CompareTo(func(y)));
    }
}
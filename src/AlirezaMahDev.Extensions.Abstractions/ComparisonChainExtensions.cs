namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparisonChainExtensions
{
    extension<T>(ComparisonChain<T>)
    {
        public static ComparisonChain<T> Order(Comparison<T> comparison) =>
            new(comparison);

        public static ComparisonChain<T> OrderDescending(Comparison<T> comparison) =>
            new((x, y) => comparison(y, x));

        public static ComparisonChain<T> OrderBy<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey> =>
            ComparisonChain<T>.Order((x, y) => func(x).CompareTo(func(y)));

        public static ComparisonChain<T> OrderByDescending<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey> =>
            ComparisonChain<T>.OrderDescending((x, y) => func(x).CompareTo(func(y)));
    }

    extension<T>(ComparisonChain<T> unwrap)
    {
        public ComparisonWrap<ComparisonChain<T>, T> Wrap() =>
            new(unwrap);
    }

    extension<TComparisonChain, T>(ComparisonWrap<TComparisonChain, T> wrap)
        where TComparisonChain : struct, IComparisonChain<T>
    {
        public ComparisonWrap<TComparisonChain, T> Then(Comparison<T> comparison) =>
            new(wrap.UnWrap with
            {
                Comparison = (x, y) => wrap.UnWrap.Comparison(x, y) is var z && z != 0 ? z : comparison(x, y)
            });

        public ComparisonWrap<TComparisonChain, T> ThenDescending(Comparison<T> comparison) =>
            new(wrap.UnWrap with
            {
                Comparison = (x, y) => wrap.UnWrap.Comparison(x, y) is var z && z != 0 ? z : comparison(y, x)
            });

        public ComparisonWrap<TComparisonChain, T> ThenBy<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey> =>
            wrap.Then((x, y) => func(x).CompareTo(func(y)));

        public ComparisonWrap<TComparisonChain, T> ThenByDescending<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey> =>
            wrap.ThenDescending((x, y) => func(x).CompareTo(func(y)));
    }
}
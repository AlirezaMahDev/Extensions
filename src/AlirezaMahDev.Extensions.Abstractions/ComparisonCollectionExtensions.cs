namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparisonCollectionExtensions
{
    extension<T>(ComparisonCollection<T> unwrap)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<ComparisonCollection<T>, T> Wrap()
        {
            return new(unwrap);
        }
    }

    extension<TComparisonCollection, T>(ComparisonWrap<TComparisonCollection, T> wrap)
        where TComparisonCollection : struct, IComparisonCollection<T>
        where T : allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonCollection, T> With(Comparison<T> comparison)
        {
            return new(wrap.UnWrap with { Enumerable = wrap.UnWrap.Enumerable.Append(comparison) });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonCollection, T> WithDescending(Comparison<T> comparison)
        {
            return new(wrap.UnWrap with { Enumerable = wrap.UnWrap.Enumerable.Append((x, y) => comparison(y, x)) });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonCollection, T> With(Comparison<T> comparison,
            ComparisonBuilder<ComparisonChain<T>, T> builder)
        {
            return wrap.With(builder(ComparisonChain<T>.ChainOrder(comparison)).UnWrap.Comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonCollection, T> WithDescending(Comparison<T> comparison,
            ComparisonBuilder<ComparisonChain<T>, T> builder)
        {
            return wrap.With(builder(ComparisonChain<T>.ChainOrderDescending(comparison)).UnWrap.Comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonCollection, T> WithBy<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey>
        {
            return wrap.With((x, y) => func(x).CompareTo(func(y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonCollection, T> WithByDescending<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey>
        {
            return wrap.WithDescending((x, y) => func(x).CompareTo(func(y)));
        }
    }
}
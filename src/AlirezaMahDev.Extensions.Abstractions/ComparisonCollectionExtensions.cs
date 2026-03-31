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
        public ComparisonWrap<TComparisonCollection, T> With(ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            return new(wrap.UnWrap with { Enumerable = wrap.UnWrap.Enumerable.Append(readOnlyComparison) });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonCollection, T> WithDescending(ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            return new(wrap.UnWrap with
            {
                Enumerable =
                wrap.UnWrap.Enumerable.Append((scoped ref readonly x, scoped ref readonly y) =>
                    readOnlyComparison(in y, in x))
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonCollection, T> With(ScopedRefReadOnlyComparison<T> readOnlyComparison,
            ComparisonBuilder<ScopedComparisonChain<T>, T> builder)
        {
            return wrap.With(builder(ScopedComparisonChain<T>.ChainOrder(readOnlyComparison)).UnWrap.Comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonCollection, T> WithDescending(ScopedRefReadOnlyComparison<T> readOnlyComparison,
            ComparisonBuilder<ScopedComparisonChain<T>, T> builder)
        {
            return wrap.With(builder(ScopedComparisonChain<T>.ChainOrderDescending(readOnlyComparison)).UnWrap.Comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonCollection, T> WithBy<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey>
        {
            return wrap.With((scoped ref readonly x, scoped ref readonly y) => func(x).CompareTo(func(y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonCollection, T> WithByDescending<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey>
        {
            return wrap.WithDescending((scoped ref readonly x, scoped ref readonly y) => func(x).CompareTo(func(y)));
        }
    }
}
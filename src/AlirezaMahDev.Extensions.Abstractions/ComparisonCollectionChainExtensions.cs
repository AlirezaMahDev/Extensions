namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparisonCollectionChainExtensions
{
    extension<T>(ComparisonCollectionChain<T>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonCollectionChain<T> OrderBy(ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            return new([], readOnlyComparison, readOnlyComparison, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonCollectionChain<T> OrderByDescending(ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            return new([],
                (scoped ref readonly x, scoped ref readonly y) => readOnlyComparison(in y, in x),
                (scoped ref readonly x, scoped ref readonly y) => readOnlyComparison(in y, in x),
                null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonCollectionChain<T> OrderBy(IEnumerable<ScopedRefReadOnlyComparison<T>> enumerable,
            ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            return new(enumerable, readOnlyComparison, readOnlyComparison, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonCollectionChain<T> OrderByDescending(IEnumerable<ScopedRefReadOnlyComparison<T>> enumerable,
            ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            return new(enumerable,
                (scoped ref readonly x, scoped ref readonly y) => readOnlyComparison(in y, in x),
                (scoped ref readonly x, scoped ref readonly y) => readOnlyComparison(in y, in x),
                null);
        }
    }

    extension<T>(ComparisonCollectionChain<T> unwrap)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<ComparisonCollectionChain<T>, T> Wrap()
        {
            return new(unwrap);
        }
    }
}
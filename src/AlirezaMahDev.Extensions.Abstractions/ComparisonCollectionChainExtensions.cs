namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparisonCollectionChainExtensions
{
    extension<T>(ComparisonCollectionChain<T>)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonCollectionChain<T> OrderBy(Comparison<T> comparison)
        {
            return new([], comparison, comparison, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonCollectionChain<T> OrderByDescending(Comparison<T> comparison)
        {
            return new([], (x, y) => comparison(y, x), (x, y) => comparison(y, x), null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonCollectionChain<T> OrderBy(IEnumerable<Comparison<T>> enumerable,
            Comparison<T> comparison)
        {
            return new(enumerable, comparison, comparison, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonCollectionChain<T> OrderByDescending(IEnumerable<Comparison<T>> enumerable,
            Comparison<T> comparison)
        {
            return new(enumerable, (x, y) => comparison(y, x), (x, y) => comparison(y, x), null);
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
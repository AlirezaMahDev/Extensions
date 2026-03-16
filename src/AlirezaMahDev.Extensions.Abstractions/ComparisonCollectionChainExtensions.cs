namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparisonCollectionChainExtensions
{
    extension<T>(ComparisonCollectionChain<T>)
    {
        public static ComparisonCollectionChain<T> OrderBy(Comparison<T> comparison)
        {
            return new([], comparison, comparison, null);
        }

        public static ComparisonCollectionChain<T> OrderByDescending(Comparison<T> comparison)
        {
            return new([], (x, y) => comparison(y, x), (x, y) => comparison(y, x), null);
        }

        public static ComparisonCollectionChain<T> OrderBy(IEnumerable<Comparison<T>> enumerable,
            Comparison<T> comparison)
        {
            return new(enumerable, comparison, comparison, null);
        }

        public static ComparisonCollectionChain<T> OrderByDescending(IEnumerable<Comparison<T>> enumerable,
            Comparison<T> comparison)
        {
            return new(enumerable, (x, y) => comparison(y, x), (x, y) => comparison(y, x), null);
        }
    }

    extension<T>(ComparisonCollectionChain<T> unwrap)
    {
        public ComparisonWrap<ComparisonCollectionChain<T>, T> Wrap()
        {
            return new(unwrap);
        }
    }
}
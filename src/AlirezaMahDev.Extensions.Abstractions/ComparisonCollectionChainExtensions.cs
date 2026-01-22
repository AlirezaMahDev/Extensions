namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparisonCollectionChainExtensions
{
    extension<T>(ComparisonCollectionChain<T>)
    {
        public static ComparisonCollectionChain<T> OrderBy(Comparison<T> comparison) =>
            new([], comparison);

        public static ComparisonCollectionChain<T> OrderByDescending(Comparison<T> comparison) =>
            new([], (x, y) => comparison(y, x));

        public static ComparisonCollectionChain<T> OrderBy(IEnumerable<Comparison<T>> enumerable,
            Comparison<T> comparison) =>
            new(enumerable, comparison);

        public static ComparisonCollectionChain<T> OrderByDescending(IEnumerable<Comparison<T>> enumerable,
            Comparison<T> comparison) =>
            new(enumerable, (x, y) => comparison(y, x));
    }

    extension<T>(ComparisonCollectionChain<T> unwrap)
    {
        public ComparisonWrap<ComparisonCollectionChain<T>, T> Wrap() =>
            new(unwrap);
    }
}
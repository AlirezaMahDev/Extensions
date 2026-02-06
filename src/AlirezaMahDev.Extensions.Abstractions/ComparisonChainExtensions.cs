namespace AlirezaMahDev.Extensions.Abstractions;

public static class ComparisonChainExtensions
{
    extension<T>(ComparisonChain<T>)
    {
        public static ComparisonWrap<ComparisonChain<T>, T> ChainOrder(Comparison<T> comparison) =>
            new ComparisonChain<T>(comparison, comparison, null).Wrap();

        public static ComparisonWrap<ComparisonChain<T>, T> ChainOrderDescending(Comparison<T> comparison) =>
            new ComparisonChain<T>((x, y) => comparison(y, x), (x, y) => comparison(y, x), null).Wrap();

        public static ComparisonWrap<ComparisonChain<T>, T> ChainOrderBy<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey> =>
            ComparisonChain<T>.ChainOrder((x, y) => func(x).CompareTo(func(y)));

        public static ComparisonWrap<ComparisonChain<T>, T> ChainOrderByDescending<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey> =>
            ComparisonChain<T>.ChainOrderDescending((x, y) => func(x).CompareTo(func(y)));
    }

    extension<T>(ComparisonChain<T> unwrap)
    {
        public ComparisonWrap<ComparisonChain<T>, T> Wrap() =>
            new(unwrap);
    }

    extension<TComparisonChain, T>(ComparisonWrap<TComparisonChain, T> wrap)
        where TComparisonChain : struct, IComparisonChain<T>
    {
        public ComparisonWrap<TComparisonChain, T> ChainOrder(Comparison<T> comparison) =>
            new(wrap.UnWrap with
            {
                Comparison = (x, y) => wrap.UnWrap.Comparison(x, y) is var z && z != 0 ? z : comparison(x, y),
                CurrentComparison = (x, y) => comparison(x, y),
                PreviousComparisonChain = wrap.UnWrap
            });

        public ComparisonWrap<TComparisonChain, T> ChainOrderDescending(Comparison<T> comparison) =>
            new(wrap.UnWrap with
            {
                Comparison = (x, y) => wrap.UnWrap.Comparison(x, y) is var z && z != 0 ? z : comparison(y, x),
                CurrentComparison = (x, y) => comparison(y, x),
                PreviousComparisonChain = wrap.UnWrap
            });

        public ComparisonWrap<TComparisonChain, T> ChainOrderBy<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey> =>
            wrap.ChainOrder((x, y) => func(x).CompareTo(func(y)));

        public ComparisonWrap<TComparisonChain, T> ChainOrderByDescending<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey> =>
            wrap.ChainOrderDescending((x, y) => func(x).CompareTo(func(y)));

        public IEnumerable<IComparisonChain<T>> GetComparisonChains()
        {
            var stack = new Stack<IComparisonChain<T>>();
            IComparisonChain<T>? current = wrap.UnWrap;
            while (current is not null)
            {
                stack.Push(current);
                current = current.PreviousComparisonChain;
            }
            return stack;
        }
    }
}
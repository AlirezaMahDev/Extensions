namespace AlirezaMahDev.Extensions.Abstractions;

public static class ScopedComparisonChainExtensions
{
    extension<T>(ScopedComparisonChain<T>)
        where T : allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonWrap<ScopedComparisonChain<T>, T> ChainOrder(ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            return new ScopedComparisonChain<T>(readOnlyComparison, readOnlyComparison, null).Wrap();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonWrap<ScopedComparisonChain<T>, T> ChainOrderDescending(
            ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            return new ScopedComparisonChain<T>(
                [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                (scoped ref readonly x, scoped ref readonly y) => readOnlyComparison(in y, in x),
                (scoped ref readonly x, scoped ref readonly y) => readOnlyComparison(in y, in x),
                null).Wrap();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonWrap<ScopedComparisonChain<T>, T> ChainOrderBy<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey>
        {
            return ScopedComparisonChain<T>.ChainOrder([MethodImpl(MethodImplOptions.AggressiveInlining |
                                                             MethodImplOptions.AggressiveOptimization)]
                (scoped ref readonly x, scoped ref readonly y) => func(x).CompareTo(func(y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ComparisonWrap<ScopedComparisonChain<T>, T> ChainOrderByDescending<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey>
        {
            return ScopedComparisonChain<T>.ChainOrderDescending([MethodImpl(MethodImplOptions.AggressiveInlining |
                                                                       MethodImplOptions.AggressiveOptimization)]
                (scoped ref readonly x, scoped ref readonly y) => func(x).CompareTo(func(y)));
        }
    }

    extension<T>(ScopedComparisonChain<T> unwrap)
        where T : allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<ScopedComparisonChain<T>, T> Wrap()
        {
            return new(unwrap);
        }
    }

    extension<TComparisonChain, T>(ComparisonWrap<TComparisonChain, T> wrap)
        where TComparisonChain : struct, IComparisonChain<T>
        where T : allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonChain, T> ChainOrder(ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            return new(wrap.UnWrap with
            {
                Comparison =
                [MethodImpl(MethodImplOptions.AggressiveInlining |
                            MethodImplOptions.AggressiveOptimization)]
                (scoped ref readonly x, scoped ref readonly y) =>
                    wrap.UnWrap.Comparison(in x, in y) is var z && z != 0 ? z : readOnlyComparison(in x, in y),
                CurrentComparison = readOnlyComparison,
                PreviousComparisonChain = wrap.UnWrap
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonChain, T> ChainOrderDescending(ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            return new(wrap.UnWrap with
            {
                Comparison =
                [MethodImpl(MethodImplOptions.AggressiveInlining |
                            MethodImplOptions.AggressiveOptimization)]
                (scoped ref readonly x, scoped ref readonly y) =>
                    wrap.UnWrap.Comparison(in x, in y) is var z && z != 0
                        ? z
                        : readOnlyComparison(in y, in x),
                CurrentComparison =
                [MethodImpl(MethodImplOptions.AggressiveInlining |
                            MethodImplOptions.AggressiveOptimization)]
                (scoped ref readonly x, scoped ref readonly y) => readOnlyComparison(in y, in x),
                PreviousComparisonChain = wrap.UnWrap
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonChain, T> ChainOrderBy<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey>
        {
            return wrap.ChainOrder([MethodImpl(MethodImplOptions.AggressiveInlining |
                                               MethodImplOptions.AggressiveOptimization)]
                (scoped ref readonly x, scoped ref readonly y) => func(x).CompareTo(func(y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonChain, T> ChainOrderByDescending<TKey>(Func<T, TKey> func)
            where TKey : IComparable<TKey>
        {
            return wrap.ChainOrderDescending([MethodImpl(MethodImplOptions.AggressiveInlining |
                                                         MethodImplOptions.AggressiveOptimization)]
                (scoped ref readonly x, scoped ref readonly y) => func(x).CompareTo(func(y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<IComparisonChain<T>> GetComparisonChains()
        {
            Stack<IComparisonChain<T>> stack = new();
            IComparisonChain<T>? current = wrap.UnWrap;
            while (current is not null)
            {
                stack.Push(current);
                current = current.PreviousComparisonChain;
            }

            return stack;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ComparisonWrap<TComparisonChain, T> Merge()
        {
            return new(wrap.UnWrap with
            {
                Comparison = wrap.UnWrap.Comparison,
                CurrentComparison = wrap.UnWrap.Comparison,
                PreviousComparisonChain = null
            });
        }
    }
}
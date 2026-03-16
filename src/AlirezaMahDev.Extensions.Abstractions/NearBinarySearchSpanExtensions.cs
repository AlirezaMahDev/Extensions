using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class NearBinarySearchSpanExtensions
{
    extension<T>(ReadOnlyMemory<T> readonlyMemory)
    {
        [MustDisposeResource]
        public MemoryList<ReadOnlyMemory<T>> Near(T value,
            ComparisonChain<T> comparisonChain,
            int depth)
        {
            MemoryList<ReadOnlyMemory<T>> result = [readonlyMemory];
            foreach (IComparisonChain<T> comparison in comparisonChain.Wrap().GetComparisonChains())
            {
                MemoryList<ReadOnlyMemory<T>> newResult = [];
                foreach (ReadOnlyMemory<T> item in result)
                {
                    item.NearCore(newResult, value, comparison.CurrentComparison, depth);
                }

                result.Dispose();
                result = newResult;
            }

            return result;
        }

        private void NearCore(MemoryList<ReadOnlyMemory<T>> result,
            T value,
            Comparison<T> comparison,
            int depth)
        {
            if (readonlyMemory.IsEmpty)
            {
                return;
            }

            BinarySearchRange binarySearchRange = readonlyMemory.Span.BinarySearchRange(value, comparison);
            if (binarySearchRange.TryGetRange(out Range range))
            {
                result.Add(readonlyMemory[range]);
            }

            if (depth == 0)
            {
                return;
            }

            int before, after;
            if (binarySearchRange.Start < 0)
            {
                int define = ~binarySearchRange.Start;
                before = define - 1;
                after = define;
            }
            else
            {
                before = binarySearchRange.Start - 1;
                after = binarySearchRange.End + 1;
            }


            if (before >= 0 && before < readonlyMemory.Length)
            {
                readonlyMemory[..(before + 1)]
                    .NearCore(result, readonlyMemory.Span[before], comparison, depth - 1);
            }

            if (after >= 0 && after < readonlyMemory.Length)
            {
                readonlyMemory[after..]
                    .NearCore(result, readonlyMemory.Span[after], comparison, depth - 1);
            }
        }

        [MustDisposeResource]
        public MemoryList<ReadOnlyMemory<T>> Near<TBridge>(TBridge value,
            Func<T, TBridge> func,
            ComparisonChain<TBridge> comparisonChain,
            int depth)
            where TBridge : allows ref struct
        {
            MemoryList<ReadOnlyMemory<T>> result = [readonlyMemory];
            foreach (IComparisonChain<TBridge> comparison in comparisonChain.Wrap().GetComparisonChains())
            {
                MemoryList<ReadOnlyMemory<T>> newResult = [];
                foreach (ReadOnlyMemory<T> item in result)
                {
                    item.NearCore(newResult, value, func, comparison.CurrentComparison, depth);
                }

                result.Dispose();
                result = newResult;
            }

            return result;
        }

        private void NearCore<TBridge>(MemoryList<ReadOnlyMemory<T>> result,
            TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison,
            int depth)
            where TBridge : allows ref struct
        {
            if (readonlyMemory.IsEmpty)
            {
                return;
            }

            BinarySearchRange binarySearchRange = readonlyMemory.Span.BinarySearchRange(value, func, comparison);
            if (binarySearchRange.TryGetRange(out Range range))
            {
                result.Add(readonlyMemory[range]);
            }

            if (depth == 0)
            {
                return;
            }

            int before, after;
            if (binarySearchRange.Start < 0)
            {
                int define = ~binarySearchRange.Start;
                before = define - 1;
                after = define;
            }
            else
            {
                before = binarySearchRange.Start - 1;
                after = binarySearchRange.End + 1;
            }


            if (before >= 0 && before < readonlyMemory.Length)
            {
                readonlyMemory[..(before + 1)]
                    .NearCore(result, func(readonlyMemory.Span[before]), func, comparison, depth - 1);
            }

            if (after >= 0 && after < readonlyMemory.Length)
            {
                readonlyMemory[after..]
                    .NearCore(result, func(readonlyMemory.Span[after]), func, comparison, depth - 1);
            }
        }
    }

    extension<T>(Memory<T> memory)
    {
        [MustDisposeResource]
        public MemoryList<ReadOnlyMemory<T>> Near(T value, ComparisonChain<T> comparisonChainWrap, int depth)
        {
            return ((ReadOnlyMemory<T>)memory).Near(value, comparisonChainWrap, depth);
        }

        [MustDisposeResource]
        public MemoryList<ReadOnlyMemory<T>> Near<TBridge>(TBridge value,
            Func<T, TBridge> func,
            ComparisonChain<TBridge> comparisonChainWrap,
            int depth)
            where TBridge : allows ref struct
        {
            return ((ReadOnlyMemory<T>)memory).Near(value, func, comparisonChainWrap, depth);
        }
    }
}
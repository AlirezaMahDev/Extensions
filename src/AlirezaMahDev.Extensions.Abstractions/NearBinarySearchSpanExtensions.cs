using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class NearBinarySearchSpanExtensions
{
    extension<T>(ReadOnlyMemory<T> readonlyMemory)
    {
        public IEnumerable<T> Near(T value,
            ComparisonChain<T> comparisonChain,
            int depth)
        {
            List<ReadOnlyMemory<T>> result = [readonlyMemory];
            foreach (var comparison in comparisonChain.Wrap().GetComparisonChains())
            {
                List<ReadOnlyMemory<T>> newResult = [];
                foreach (var item in result)
                {
                    newResult.AddRange(item.NearCore(value, comparison.CurrentComparison, depth));
                }

                result = newResult;
            }

            foreach (var item in result)
            {
                foreach (var subItem in MemoryMarshal.ToEnumerable(item))
                {
                    yield return subItem;
                }
            }
        }

        private IEnumerable<ReadOnlyMemory<T>> NearCore(T value,
            Comparison<T> comparison,
            int depth)
        {
            if (readonlyMemory.IsEmpty)
            {
                yield break;
            }

            var binarySearchRange = readonlyMemory.Span.BinarySearchRange(value, comparison);
            if (binarySearchRange.TryGetRange(out var range))
            {
                yield return readonlyMemory[range];
            }

            if (depth == 0)
            {
                yield break;
            }

            int before, after;
            if (binarySearchRange.Start < 0)
            {
                var define = ~binarySearchRange.Start;
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
                foreach (var item in readonlyMemory[..(before + 1)]
                             .NearCore(readonlyMemory.Span[before], comparison, depth - 1))
                {
                    yield return item;
                }
            }

            if (after >= 0 && after < readonlyMemory.Length)
            {
                foreach (var item in readonlyMemory[after..]
                             .NearCore(readonlyMemory.Span[after], comparison, depth - 1))
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<T> Near<TBridge>(TBridge value,
            Func<T, TBridge> func,
            ComparisonChain<TBridge> comparisonChain,
            int depth)
        {
            List<ReadOnlyMemory<T>> result = [readonlyMemory];
            foreach (var comparison in comparisonChain.Wrap().GetComparisonChains())
            {
                List<ReadOnlyMemory<T>> newResult = [];
                foreach (var item in result)
                {
                    newResult.AddRange(item.NearCore(value, func, comparison.CurrentComparison, depth));
                }

                result = newResult;
            }

            foreach (var item in result)
            {
                foreach (var subItem in MemoryMarshal.ToEnumerable(item))
                {
                    yield return subItem;
                }
            }
        }

        private IEnumerable<ReadOnlyMemory<T>> NearCore<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison,
            int depth)
        {
            if (readonlyMemory.IsEmpty)
            {
                yield break;
            }

            var binarySearchRange = readonlyMemory.Span.BinarySearchRange(value, func, comparison);
            if (binarySearchRange.TryGetRange(out var range))
            {
                yield return readonlyMemory[range];
            }

            if (depth == 0)
            {
                yield break;
            }

            int before, after;
            if (binarySearchRange.Start < 0)
            {
                var define = ~binarySearchRange.Start;
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
                foreach (var item in readonlyMemory[..(before + 1)]
                             .NearCore(func(readonlyMemory.Span[before]), func, comparison, depth - 1))
                {
                    yield return item;
                }
            }

            if (after >= 0 && after < readonlyMemory.Length)
            {
                foreach (var item in readonlyMemory[after..]
                             .NearCore(func(readonlyMemory.Span[after]), func, comparison, depth - 1))
                {
                    yield return item;
                }
            }
        }
    }

    extension<T>(Memory<T> memory)
    {
        public IEnumerable<T> Near(T value, ComparisonChain<T> comparisonChainWrap, int depth) =>
            ((ReadOnlyMemory<T>)memory).Near(value, comparisonChainWrap, depth);

        public IEnumerable<T> Near<TBridge>(TBridge value,
            Func<T, TBridge> func,
            ComparisonChain<TBridge> comparisonChainWrap,
            int depth) =>
            ((ReadOnlyMemory<T>)memory).Near(value, func, comparisonChainWrap, depth);
    }
}
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class NearBinarySearchSpanExtensions
{
    extension<T>(ReadOnlyMemory<T> readonlyMemory)
    {
        public IEnumerable<T> Near(T value, ComparisonWrap<ComparisonChain<T>, T> comparisonChainWrap, int depth)
        {
            List<ReadOnlyMemory<T>> result = [readonlyMemory];
            foreach (var comparisonChain in comparisonChainWrap.GetComparisonChains())
            {
                List<ReadOnlyMemory<T>> newResult = [];
                foreach (var item in result)
                {
                    newResult.AddRange(item.NearCore(value, comparisonChain.CurrentComparison, depth));
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

        private IEnumerable<ReadOnlyMemory<T>> NearCore(T value, Comparison<T> comparison, int depth)
        {
            if (readonlyMemory.IsEmpty)
            {
                yield break;
            }

            var comparable = new ComparableComparison<T>(value, comparison);

            if (readonlyMemory.Span.BinarySearchRange(comparable) is { } binarySearchRange &&
                binarySearchRange.TryGetRange(out var range) &&
                readonlyMemory[range] is var result)
            {
                yield return result;

                if (depth > 0)
                {
                    int before = binarySearchRange.Start - 1;
                    if (before > 0)
                    {
                        foreach (var item in readonlyMemory[..(before + 1)]
                                     .NearCore(readonlyMemory.Span[before], comparison, depth - 1))
                        {
                            yield return item;
                        }
                    }

                    int after = binarySearchRange.End + 1;
                    if (after < readonlyMemory.Length)
                    {
                        foreach (var item in readonlyMemory[after..]
                                     .NearCore(readonlyMemory.Span[after], comparison, depth - 1))
                        {
                            yield return item;
                        }
                    }
                }
            }
            else
            {
                yield break;
            }
        }
    }

    extension<T>(Memory<T> memory)
    {
        public IEnumerable<T> Near(T value, ComparisonWrap<ComparisonChain<T>, T> comparisonChainWrap, int depth) =>
            ((ReadOnlyMemory<T>)memory).Near(value, comparisonChainWrap, depth);
    }
}
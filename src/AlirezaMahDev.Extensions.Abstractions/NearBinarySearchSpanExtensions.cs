namespace AlirezaMahDev.Extensions.Abstractions;

public static class NearBinarySearchSpanExtensions
{
    extension<T>(Memory<T> memory)
    {
        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public MemoryList<Memory<T>> Near(T value,
            ScopedComparisonChain<T> comparisonChain,
            int depth)
        {
            MemoryList<Memory<T>> result = [memory];
            foreach (var comparison in comparisonChain.Wrap().GetComparisonChains())
            {
                MemoryList<Memory<T>> newResult = [];
                foreach (var item in result)
                {
                    item.NearCore(newResult, value, comparison.CurrentComparison, depth);
                }

                result.Dispose();
                result = newResult;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private void NearCore(MemoryList<Memory<T>> result,
            T value,
            ScopedRefReadOnlyComparison<T> readOnlyComparison,
            int depth)
        {
            if (memory.IsEmpty)
            {
                return;
            }

            var binarySearchRange = memory.Span.BinarySearchRange(value, readOnlyComparison);
            if (binarySearchRange.TryGetRange(out var range))
            {
                result.Add(memory[range]);
            }

            if (depth == 0)
            {
                return;
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


            if (before >= 0 && before < memory.Length)
            {
                memory[..(before + 1)]
                    .NearCore(result, memory.Span[before], readOnlyComparison, depth - 1);
            }

            if (after >= 0 && after < memory.Length)
            {
                memory[after..]
                    .NearCore(result, memory.Span[after], readOnlyComparison, depth - 1);
            }
        }

        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public MemoryList<Memory<T>> Near<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedComparisonChain<TBridge> comparisonChain,
            int depth)
            where TBridge : allows ref struct
        {
            MemoryList<Memory<T>> result = [memory];
            foreach (var comparison in comparisonChain.Wrap().GetComparisonChains())
            {
                MemoryList<Memory<T>> newResult = [];
                foreach (var item in result)
                {
                    item.NearCore(newResult, ref value, func, comparison.CurrentComparison, depth);
                }

                result.Dispose();
                result = newResult;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private void NearCore<TBridge>(MemoryList<Memory<T>> result,
            ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison,
            int depth)
            where TBridge : allows ref struct
        {
            if (memory.IsEmpty)
            {
                return;
            }

            var span = memory.Span;
            var binarySearchRange = span.BinarySearchRange(ref value, func, readOnlyComparison);
            if (binarySearchRange.TryGetRange(out var range))
            {
                result.Add(memory[range]);
            }

            if (depth == 0)
            {
                return;
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


            if (before >= 0 && before < memory.Length)
            {
                var bridge = func(ref span[before]);
                memory[..(before + 1)]
                    .NearCore(result, ref bridge, func, readOnlyComparison, depth - 1);
            }

            if (after >= 0 && after < memory.Length)
            {
                var bridge = func(ref span[after]);
                memory[after..]
                    .NearCore(result, ref bridge, func, readOnlyComparison, depth - 1);
            }
        }
    }
}
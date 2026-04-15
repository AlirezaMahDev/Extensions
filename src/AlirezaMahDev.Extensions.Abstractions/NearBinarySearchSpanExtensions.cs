namespace AlirezaMahDev.Extensions.Abstractions;

public static class NearBinarySearchSpanExtensions
{
    extension(Range)
    {
        public static Range Merge(Range parent, Range child)
        {
            return new(parent.Start.Value + child.Start.Value, parent.Start.Value + child.End.Value);
        }
    }

    extension<TSelf, T>(TSelf refReadOnlyBlock)
        where TSelf : IRefReadOnlyBlock<TSelf, T>, allows ref struct
    {
        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public NativeRefList<Range> Near(T value,
            ScopedComparisonChain<T> comparisonChain,
            int depth)
        {
            NativeRefList<Range> result = [new(0, ^0)];
            foreach (var comparison in comparisonChain.Wrap().GetComparisonChains())
            {
                NativeRefList<Range> newResult = [];
                foreach (var item in result)
                {
                    refReadOnlyBlock[item].NearCore(item, newResult, value, comparison.CurrentComparison, depth);
                }

                result.Dispose();
                result = newResult;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private void NearCore(
            Range item,
            NativeRefList<Range> result,
            T value,
            ScopedRefReadOnlyComparison<T> readOnlyComparison,
            int depth)
        {
            if (refReadOnlyBlock.IsEmpty)
            {
                return;
            }

            var binarySearchRange = refReadOnlyBlock.BinarySearchRange(value, readOnlyComparison);
            if (binarySearchRange.TryGetRange(out var range))
            {
                result.Add(Range.Merge(item, range));
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


            if (before >= 0 && before < refReadOnlyBlock.Length)
            {
                Range start = ..(before + 1);
                refReadOnlyBlock[start]
                    .NearCore(Range.Merge(item, start), result, refReadOnlyBlock[before], readOnlyComparison, depth - 1);
            }

            if (after >= 0 && after < refReadOnlyBlock.Length)
            {
                Range start = after..;
                refReadOnlyBlock[start]
                    .NearCore(Range.Merge(item, start), result, refReadOnlyBlock[after], readOnlyComparison, depth - 1);
            }
        }

        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public NativeRefList<Range> Near<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedComparisonChain<TBridge> comparisonChain,
            int depth)
            where TBridge : allows ref struct
        {
            NativeRefList<Range> result = [new(0, refReadOnlyBlock.Length)];
            foreach (var comparison in comparisonChain.Wrap().GetComparisonChains())
            {
                NativeRefList<Range> newResult = [];
                foreach (var item in result)
                {
                    refReadOnlyBlock[item].NearCore(item, ref newResult, ref value, func, comparison.CurrentComparison, depth);
                }

                result.Dispose();
                result = newResult;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private void NearCore<TBridge>(
            Range item,
            ref NativeRefList<Range> result,
            ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison,
            int depth)
            where TBridge : allows ref struct
        {
            if (refReadOnlyBlock.IsEmpty)
            {
                return;
            }

            var binarySearchRange = refReadOnlyBlock.BinarySearchRange(ref value, func, readOnlyComparison);
            if (binarySearchRange.TryGetRange(out var range))
            {
                result.Add(Range.Merge(item, range));
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


            if (before >= 0 && before < refReadOnlyBlock.Length)
            {
                var bridge = func(in refReadOnlyBlock[before]);
                Range start = ..(before + 1);
                refReadOnlyBlock[start]
                    .NearCore(Range.Merge(item, start), ref result, ref bridge, func, readOnlyComparison, depth - 1);
            }

            if (after >= 0 && after < refReadOnlyBlock.Length)
            {
                var bridge = func(in refReadOnlyBlock[after]);
                Range start = after..;
                refReadOnlyBlock[start]
                    .NearCore(Range.Merge(item, start), ref result, ref bridge, func, readOnlyComparison, depth - 1);
            }
        }
    }
}
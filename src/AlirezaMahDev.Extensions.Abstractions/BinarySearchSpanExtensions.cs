namespace AlirezaMahDev.Extensions.Abstractions;

public static class BinarySearchSpanExtensions
{
    extension<T>(ReadOnlySpan<T> readonlySpan)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound<TComparable>(in TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            var lower = readonlySpan.LowerBound(value);

            return lower < readonlySpan.Length && value.CompareTo(readonlySpan[lower]) == 0
                ? lower
                : ~lower;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound(in T value, Comparison<T> comparison)
        {
            return readonlySpan.BinarySearchLowerBound(new ComparableComparison<T>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound<TComparer>(in T value, in TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.BinarySearchLowerBound(new ComparableComparer<T, TComparer>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound<TBridge>(in TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.BinarySearchLowerBound(
                new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound<TBridge, TComparer>(in TBridge value,
            Func<T, TBridge> func,
            in TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.BinarySearchLowerBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }

        public int BinarySearchUpperBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            var lower = readonlySpan.LowerBound(value);

            if (lower >= readonlySpan.Length || value.CompareTo(readonlySpan[lower]) != 0)
            {
                return ~lower;
            }

            var upper = readonlySpan[lower..].UpperBound(value) + lower;

            return upper - 1;
        }

        public int BinarySearchUpperBound(T value, Comparison<T> comparison)
        {
            return readonlySpan.BinarySearchUpperBound(new ComparableComparison<T>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchUpperBound<TComparer>(T value, in TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.BinarySearchUpperBound(new ComparableComparer<T, TComparer>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchUpperBound<TBridge>(in TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.BinarySearchUpperBound(
                new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchUpperBound<TBridge, TComparer>(in TBridge value,
            Func<T, TBridge> func,
            in TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.BinarySearchUpperBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int LowerBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            var lo = 0;
            var hi = readonlySpan.Length;

            while (lo < hi)
            {
                var mid = lo + ((hi - lo) >> 1);

                if (value.CompareTo(readonlySpan[mid]) > 0)
                {
                    lo = mid + 1;
                }
                else
                {
                    hi = mid;
                }
            }

            return lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int LowerBound(T value, Comparison<T> comparison)
        {
            return readonlySpan.LowerBound(new ComparableComparison<T>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int LowerBound<TComparer>(T value, in TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.LowerBound(new ComparableComparer<T, TComparer>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int LowerBound<TBridge>(in TBridge value, Func<T, TBridge> func, Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.LowerBound(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        public int LowerBound<TBridge, TComparer>(in TBridge value, Func<T, TBridge> func, in TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.LowerBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            var lo = 0;
            var hi = readonlySpan.Length;

            while (lo < hi)
            {
                var mid = lo + ((hi - lo) >> 1);

                if (value.CompareTo(readonlySpan[mid]) >= 0)
                {
                    lo = mid + 1;
                }
                else
                {
                    hi = mid;
                }
            }

            return lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound(T value, Comparison<T> comparison)
        {
            return readonlySpan.UpperBound(new ComparableComparison<T>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound<TComparer>(T value, in TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.UpperBound(new ComparableComparer<T, TComparer>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound<TBridge>(in TBridge value, Func<T, TBridge> func, Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.UpperBound(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound<TBridge, TComparer>(in TBridge value, Func<T, TBridge> func, in TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.UpperBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BinarySearchRange BinarySearchRange<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            var lower = readonlySpan.BinarySearchLowerBound(value);

            if (lower < 0)
            {
                return new(lower, lower);
            }

            var upper = readonlySpan[lower..].BinarySearchUpperBound(value);

            if (upper < 0)
            {
                upper = ~upper - 1;
            }

            return new(lower, lower + upper);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BinarySearchRange BinarySearchRange(T value, Comparison<T> comparison)
        {
            return readonlySpan.BinarySearchRange(new ComparableComparison<T>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BinarySearchRange BinarySearchRange<TComparer>(T value, in TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.BinarySearchRange(new ComparableComparer<T, TComparer>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BinarySearchRange BinarySearchRange<TBridge>(in TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.BinarySearchRange(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        public BinarySearchRange BinarySearchRange<TBridge, TComparer>(in TBridge value,
            Func<T, TBridge> func,
            in TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.BinarySearchRange(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TComparable>(TComparable value, out ReadOnlySpan<T> values)
            where TComparable : IComparable<T>, allows ref struct
        {
            var binarySearchRange = readonlySpan.BinarySearchRange(value);
            if (binarySearchRange.TryGetRange(out var range))
            {
                values = readonlySpan[range];
                return true;
            }

            values = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice(T value, Comparison<T> comparison, out ReadOnlySpan<T> values)
        {
            return readonlySpan.TryBinarySearchRangeSlice(new ComparableComparison<T>(value, comparison), out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TComparer>(T value, in TComparer comparison, out ReadOnlySpan<T> values)
            where TComparer : IComparer<T>
        {
            return readonlySpan.TryBinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison),
                out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TBridge>(in TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison,
            out ReadOnlySpan<T> values)
            where TBridge : allows ref struct
        {
            return readonlySpan.TryBinarySearchRangeSlice(
                new ComparableComparisonBridge<T, TBridge>(value, func, comparison),
                out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TBridge, TComparer>(in TBridge value,
            Func<T, TBridge> func,
            in TComparer comparison,
            out ReadOnlySpan<T> values)
            where TComparer : IComparer<TBridge>
            where TBridge : allows ref struct
        {
            return readonlySpan.TryBinarySearchRangeSlice(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison),
                out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpan<T> BinarySearchRangeSlice<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            return readonlySpan.TryBinarySearchRangeSlice(value, out var values)
                ? values
                : throw new("not found in slice");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpan<T> BinarySearchRangeSlice(T value, Comparison<T> comparison)
        {
            return readonlySpan.BinarySearchRangeSlice(new ComparableComparison<T>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpan<T> BinarySearchRangeSlice<TComparer>(T value, in TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.BinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpan<T> BinarySearchRangeSlice<TBridge>(in TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.BinarySearchRangeSlice(
                new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        public ReadOnlySpan<T> BinarySearchRangeSlice<TBridge, TComparer>(in TBridge value,
            Func<T, TBridge> func,
            in TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.BinarySearchRangeSlice(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }
    }

    extension<T>(Span<T> span)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TComparable>(TComparable value, out Span<T> values)
            where TComparable : IComparable<T>, allows ref struct
        {
            var binarySearchRange = span.BinarySearchRange(value);
            if (binarySearchRange.TryGetRange(out var range))
            {
                values = span[range];
                return true;
            }

            values = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice(T value, Comparison<T> comparison, out Span<T> values)
        {
            return span.TryBinarySearchRangeSlice(new ComparableComparison<T>(value, comparison), out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TComparer>(T value, in TComparer comparison, out Span<T> values)
            where TComparer : IComparer<T>
        {
            return span.TryBinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison), out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TBridge>(in TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison,
            out ReadOnlySpan<T> values)
            where TBridge : allows ref struct
        {
            return span.TryBinarySearchRangeSlice(new ComparableComparisonBridge<T, TBridge>(value, func, comparison),
                out values);
        }

        public bool TryBinarySearchRangeSlice<TBridge, TComparer>(in TBridge value,
            Func<T, TBridge> func,
            in TComparer comparison,
            out ReadOnlySpan<T> values)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return span.TryBinarySearchRangeSlice(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison),
                out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Span<T> BinarySearchRangeSlice<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            return span.TryBinarySearchRangeSlice(value, out var values)
                ? values
                : throw new("not found in slice");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Span<T> BinarySearchRangeSlice(T value, Comparison<T> comparison)
        {
            return span.BinarySearchRangeSlice(new ComparableComparison<T>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Span<T> BinarySearchRangeSlice<TComparer>(T value, in TComparer comparison)
            where TComparer : IComparer<T>
        {
            return span.BinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Span<T> BinarySearchRangeSlice<TBridge>(in TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return span.BinarySearchRangeSlice(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Span<T> BinarySearchRangeSlice<TBridge, TComparer>(in TBridge value,
            Func<T, TBridge> func,
            in TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return span.BinarySearchRangeSlice(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }
    }
}
namespace AlirezaMahDev.Extensions.Abstractions;

public static class BinarySearchSpanExtensions
{
    extension<T>(ReadOnlySpan<T> readonlySpan)
    {
        public int BinarySearchLowerBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            int lower = readonlySpan.LowerBound(value);

            return lower < readonlySpan.Length && value.CompareTo(readonlySpan[lower]) == 0
                ? lower
                : ~lower;
        }

        public int BinarySearchLowerBound(T value, Comparison<T> comparison) =>
            readonlySpan.BinarySearchLowerBound(new ComparableComparison<T>(value, comparison));

        public int BinarySearchLowerBound<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T> =>
            readonlySpan.BinarySearchLowerBound(new ComparableComparer<T, TComparer>(value, comparison));

        public int BinarySearchLowerBound<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct =>
            readonlySpan.BinarySearchLowerBound(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));

        public int BinarySearchLowerBound<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge> =>
            readonlySpan.BinarySearchLowerBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));

        public int BinarySearchUpperBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            int lower = readonlySpan.LowerBound(value);

            if (lower >= readonlySpan.Length || value.CompareTo(readonlySpan[lower]) != 0)
                return ~lower;

            int upper = readonlySpan[lower..].UpperBound(value) + lower;

            return upper - 1;
        }

        public int BinarySearchUpperBound(T value, Comparison<T> comparison) =>
            readonlySpan.BinarySearchUpperBound(new ComparableComparison<T>(value, comparison));

        public int BinarySearchUpperBound<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T> =>
            readonlySpan.BinarySearchUpperBound(new ComparableComparer<T, TComparer>(value, comparison));

        public int BinarySearchUpperBound<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct =>
            readonlySpan.BinarySearchUpperBound(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));

        public int BinarySearchUpperBound<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge> =>
            readonlySpan.BinarySearchUpperBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));

        public int LowerBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            int lo = 0;
            int hi = readonlySpan.Length;

            while (lo < hi)
            {
                int mid = lo + (hi - lo >> 1);

                if (value.CompareTo(readonlySpan[mid]) > 0)
                    lo = mid + 1;
                else
                    hi = mid;
            }

            return lo;
        }

        public int LowerBound(T value, Comparison<T> comparison) =>
            readonlySpan.LowerBound(new ComparableComparison<T>(value, comparison));

        public int LowerBound<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T> =>
            readonlySpan.LowerBound(new ComparableComparer<T, TComparer>(value, comparison));

        public int LowerBound<TBridge>(TBridge value, Func<T, TBridge> func, Comparison<TBridge> comparison)
            where TBridge : allows ref struct =>
            readonlySpan.LowerBound(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));

        public int LowerBound<TBridge, TComparer>(TBridge value, Func<T, TBridge> func, TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge> =>
            readonlySpan.LowerBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));

        public int UpperBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            int lo = 0;
            int hi = readonlySpan.Length;

            while (lo < hi)
            {
                int mid = lo + (hi - lo >> 1);

                if (value.CompareTo(readonlySpan[mid]) >= 0)
                    lo = mid + 1;
                else
                    hi = mid;
            }

            return lo;
        }

        public int UpperBound(T value, Comparison<T> comparison) =>
            readonlySpan.UpperBound(new ComparableComparison<T>(value, comparison));

        public int UpperBound<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T> =>
            readonlySpan.UpperBound(new ComparableComparer<T, TComparer>(value, comparison));

        public int UpperBound<TBridge>(TBridge value, Func<T, TBridge> func, Comparison<TBridge> comparison)
            where TBridge : allows ref struct =>
            readonlySpan.UpperBound(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));

        public int UpperBound<TBridge, TComparer>(TBridge value, Func<T, TBridge> func, TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge> =>
            readonlySpan.UpperBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));


        public BinarySearchRange BinarySearchRange<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            int lower = readonlySpan.BinarySearchLowerBound(value);

            if (lower < 0)
                return new(lower, lower);

            int upper = readonlySpan[lower..].BinarySearchUpperBound(value) + lower;

            return new(lower, upper);
        }

        public BinarySearchRange BinarySearchRange(T value, Comparison<T> comparison) =>
            readonlySpan.BinarySearchRange(new ComparableComparison<T>(value, comparison));

        public BinarySearchRange BinarySearchRange<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T> =>
            readonlySpan.BinarySearchRange(new ComparableComparer<T, TComparer>(value, comparison));

        public BinarySearchRange BinarySearchRange<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct =>
            readonlySpan.BinarySearchRange(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));

        public BinarySearchRange BinarySearchRange<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge> =>
            readonlySpan.BinarySearchRange(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));

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

        public bool TryBinarySearchRangeSlice(T value, Comparison<T> comparison, out ReadOnlySpan<T> values) =>
            readonlySpan.TryBinarySearchRangeSlice(new ComparableComparison<T>(value, comparison), out values);

        public bool TryBinarySearchRangeSlice<TComparer>(T value, TComparer comparison, out ReadOnlySpan<T> values)
            where TComparer : IComparer<T> =>
            readonlySpan.TryBinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison), out values);

        public bool TryBinarySearchRangeSlice<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison,
            out ReadOnlySpan<T> values)
            where TBridge : allows ref struct =>
            readonlySpan.TryBinarySearchRangeSlice(new ComparableComparisonBridge<T, TBridge>(value, func, comparison),
                out values);

        public bool TryBinarySearchRangeSlice<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison,
            out ReadOnlySpan<T> values)
            where TComparer : IComparer<TBridge>
            where TBridge : allows ref struct =>
            readonlySpan.TryBinarySearchRangeSlice(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison),
                out values);

        public ReadOnlySpan<T> BinarySearchRangeSlice<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct =>
            readonlySpan.TryBinarySearchRangeSlice(value, out var values)
                ? values
                : throw new("not found in slice");

        public ReadOnlySpan<T> BinarySearchRangeSlice(T value, Comparison<T> comparison) =>
            readonlySpan.BinarySearchRangeSlice(new ComparableComparison<T>(value, comparison));

        public ReadOnlySpan<T> BinarySearchRangeSlice<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T> =>
            readonlySpan.BinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison));


        public ReadOnlySpan<T> BinarySearchRangeSlice<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct =>
            readonlySpan.BinarySearchRangeSlice(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        
        public ReadOnlySpan<T> BinarySearchRangeSlice<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge> =>
            readonlySpan.BinarySearchRangeSlice(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
    }

    extension<T>(Span<T> span)
    {
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

        public bool TryBinarySearchRangeSlice(T value, Comparison<T> comparison, out Span<T> values) =>
            span.TryBinarySearchRangeSlice(new ComparableComparison<T>(value, comparison), out values);

        public bool TryBinarySearchRangeSlice<TComparer>(T value, TComparer comparison, out Span<T> values)
            where TComparer : IComparer<T> =>
            span.TryBinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison), out values);

        public bool TryBinarySearchRangeSlice<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison,
            out ReadOnlySpan<T> values)
            where TBridge : allows ref struct =>
            span.TryBinarySearchRangeSlice(new ComparableComparisonBridge<T, TBridge>(value, func, comparison),
                out values);

        public bool TryBinarySearchRangeSlice<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison,
            out ReadOnlySpan<T> values)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge> =>
            span.TryBinarySearchRangeSlice(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison),
                out values);


        public Span<T> BinarySearchRangeSlice<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct =>
            span.TryBinarySearchRangeSlice(value, out var values)
                ? values
                : throw new("not found in slice");
        
        public Span<T> BinarySearchRangeSlice(T value, Comparison<T> comparison) =>
            span.BinarySearchRangeSlice(new ComparableComparison<T>(value, comparison));

        public Span<T> BinarySearchRangeSlice<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T> =>
            span.BinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison));
        
        public Span<T> BinarySearchRangeSlice<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct =>
            span.BinarySearchRangeSlice(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));

        public Span<T> BinarySearchRangeSlice<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge> =>
            span.BinarySearchRangeSlice(new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
    }
}
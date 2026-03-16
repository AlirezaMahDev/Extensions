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

        public int BinarySearchLowerBound(T value, Comparison<T> comparison)
        {
            return readonlySpan.BinarySearchLowerBound(new ComparableComparison<T>(value, comparison));
        }

        public int BinarySearchLowerBound<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.BinarySearchLowerBound(new ComparableComparer<T, TComparer>(value, comparison));
        }

        public int BinarySearchLowerBound<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.BinarySearchLowerBound(
                new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        public int BinarySearchLowerBound<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.BinarySearchLowerBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }

        public int BinarySearchUpperBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            int lower = readonlySpan.LowerBound(value);

            if (lower >= readonlySpan.Length || value.CompareTo(readonlySpan[lower]) != 0)
            {
                return ~lower;
            }

            int upper = readonlySpan[lower..].UpperBound(value) + lower;

            return upper - 1;
        }

        public int BinarySearchUpperBound(T value, Comparison<T> comparison)
        {
            return readonlySpan.BinarySearchUpperBound(new ComparableComparison<T>(value, comparison));
        }

        public int BinarySearchUpperBound<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.BinarySearchUpperBound(new ComparableComparer<T, TComparer>(value, comparison));
        }

        public int BinarySearchUpperBound<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.BinarySearchUpperBound(
                new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        public int BinarySearchUpperBound<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.BinarySearchUpperBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }

        public int LowerBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            int lo = 0;
            int hi = readonlySpan.Length;

            while (lo < hi)
            {
                int mid = lo + ((hi - lo) >> 1);

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

        public int LowerBound(T value, Comparison<T> comparison)
        {
            return readonlySpan.LowerBound(new ComparableComparison<T>(value, comparison));
        }

        public int LowerBound<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.LowerBound(new ComparableComparer<T, TComparer>(value, comparison));
        }

        public int LowerBound<TBridge>(TBridge value, Func<T, TBridge> func, Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.LowerBound(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        public int LowerBound<TBridge, TComparer>(TBridge value, Func<T, TBridge> func, TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.LowerBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }

        public int UpperBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            int lo = 0;
            int hi = readonlySpan.Length;

            while (lo < hi)
            {
                int mid = lo + ((hi - lo) >> 1);

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

        public int UpperBound(T value, Comparison<T> comparison)
        {
            return readonlySpan.UpperBound(new ComparableComparison<T>(value, comparison));
        }

        public int UpperBound<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.UpperBound(new ComparableComparer<T, TComparer>(value, comparison));
        }

        public int UpperBound<TBridge>(TBridge value, Func<T, TBridge> func, Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.UpperBound(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        public int UpperBound<TBridge, TComparer>(TBridge value, Func<T, TBridge> func, TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.UpperBound(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }


        public BinarySearchRange BinarySearchRange<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            int lower = readonlySpan.BinarySearchLowerBound(value);

            if (lower < 0)
            {
                return new(lower, lower);
            }

            int upper = readonlySpan[lower..].BinarySearchUpperBound(value);

            if (upper < 0)
            {
                upper = ~upper - 1;
            }

            return new(lower, lower + upper);
        }

        public BinarySearchRange BinarySearchRange(T value, Comparison<T> comparison)
        {
            return readonlySpan.BinarySearchRange(new ComparableComparison<T>(value, comparison));
        }

        public BinarySearchRange BinarySearchRange<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.BinarySearchRange(new ComparableComparer<T, TComparer>(value, comparison));
        }

        public BinarySearchRange BinarySearchRange<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.BinarySearchRange(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        public BinarySearchRange BinarySearchRange<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.BinarySearchRange(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }

        public bool TryBinarySearchRangeSlice<TComparable>(TComparable value, out ReadOnlySpan<T> values)
            where TComparable : IComparable<T>, allows ref struct
        {
            BinarySearchRange binarySearchRange = readonlySpan.BinarySearchRange(value);
            if (binarySearchRange.TryGetRange(out Range range))
            {
                values = readonlySpan[range];
                return true;
            }

            values = default;
            return false;
        }

        public bool TryBinarySearchRangeSlice(T value, Comparison<T> comparison, out ReadOnlySpan<T> values)
        {
            return readonlySpan.TryBinarySearchRangeSlice(new ComparableComparison<T>(value, comparison), out values);
        }

        public bool TryBinarySearchRangeSlice<TComparer>(T value, TComparer comparison, out ReadOnlySpan<T> values)
            where TComparer : IComparer<T>
        {
            return readonlySpan.TryBinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison),
                out values);
        }

        public bool TryBinarySearchRangeSlice<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison,
            out ReadOnlySpan<T> values)
            where TBridge : allows ref struct
        {
            return readonlySpan.TryBinarySearchRangeSlice(
                new ComparableComparisonBridge<T, TBridge>(value, func, comparison),
                out values);
        }

        public bool TryBinarySearchRangeSlice<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison,
            out ReadOnlySpan<T> values)
            where TComparer : IComparer<TBridge>
            where TBridge : allows ref struct
        {
            return readonlySpan.TryBinarySearchRangeSlice(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison),
                out values);
        }

        public ReadOnlySpan<T> BinarySearchRangeSlice<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            return readonlySpan.TryBinarySearchRangeSlice(value, out ReadOnlySpan<T> values)
                ? values
                : throw new("not found in slice");
        }

        public ReadOnlySpan<T> BinarySearchRangeSlice(T value, Comparison<T> comparison)
        {
            return readonlySpan.BinarySearchRangeSlice(new ComparableComparison<T>(value, comparison));
        }

        public ReadOnlySpan<T> BinarySearchRangeSlice<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T>
        {
            return readonlySpan.BinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison));
        }


        public ReadOnlySpan<T> BinarySearchRangeSlice<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return readonlySpan.BinarySearchRangeSlice(
                new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        public ReadOnlySpan<T> BinarySearchRangeSlice<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return readonlySpan.BinarySearchRangeSlice(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }
    }

    extension<T>(Span<T> span)
    {
        public bool TryBinarySearchRangeSlice<TComparable>(TComparable value, out Span<T> values)
            where TComparable : IComparable<T>, allows ref struct
        {
            BinarySearchRange binarySearchRange = span.BinarySearchRange(value);
            if (binarySearchRange.TryGetRange(out Range range))
            {
                values = span[range];
                return true;
            }

            values = default;
            return false;
        }

        public bool TryBinarySearchRangeSlice(T value, Comparison<T> comparison, out Span<T> values)
        {
            return span.TryBinarySearchRangeSlice(new ComparableComparison<T>(value, comparison), out values);
        }

        public bool TryBinarySearchRangeSlice<TComparer>(T value, TComparer comparison, out Span<T> values)
            where TComparer : IComparer<T>
        {
            return span.TryBinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison), out values);
        }

        public bool TryBinarySearchRangeSlice<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison,
            out ReadOnlySpan<T> values)
            where TBridge : allows ref struct
        {
            return span.TryBinarySearchRangeSlice(new ComparableComparisonBridge<T, TBridge>(value, func, comparison),
                out values);
        }

        public bool TryBinarySearchRangeSlice<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison,
            out ReadOnlySpan<T> values)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return span.TryBinarySearchRangeSlice(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison),
                out values);
        }


        public Span<T> BinarySearchRangeSlice<TComparable>(TComparable value)
            where TComparable : IComparable<T>, allows ref struct
        {
            return span.TryBinarySearchRangeSlice(value, out Span<T> values)
                ? values
                : throw new("not found in slice");
        }

        public Span<T> BinarySearchRangeSlice(T value, Comparison<T> comparison)
        {
            return span.BinarySearchRangeSlice(new ComparableComparison<T>(value, comparison));
        }

        public Span<T> BinarySearchRangeSlice<TComparer>(T value, TComparer comparison)
            where TComparer : IComparer<T>
        {
            return span.BinarySearchRangeSlice(new ComparableComparer<T, TComparer>(value, comparison));
        }

        public Span<T> BinarySearchRangeSlice<TBridge>(TBridge value,
            Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            return span.BinarySearchRangeSlice(new ComparableComparisonBridge<T, TBridge>(value, func, comparison));
        }

        public Span<T> BinarySearchRangeSlice<TBridge, TComparer>(TBridge value,
            Func<T, TBridge> func,
            TComparer comparison)
            where TBridge : allows ref struct
            where TComparer : IComparer<TBridge>
        {
            return span.BinarySearchRangeSlice(
                new ComparableComparerBridge<T, TBridge, TComparer>(value, func, comparison));
        }
    }
}
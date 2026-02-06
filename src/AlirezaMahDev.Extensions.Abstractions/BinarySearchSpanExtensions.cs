namespace AlirezaMahDev.Extensions.Abstractions;

public static class BinarySearchSpanExtensions
{
    extension<T>(ReadOnlySpan<T> readonlySpan)
    {
        public int BinarySearchLowerBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>
        {
            int lower = readonlySpan.LowerBound(value);

            return lower < readonlySpan.Length && value.CompareTo(readonlySpan[lower]) == 0
                ? lower
                : ~lower;
        }

        public int BinarySearchUpperBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>
        {
            int lower = readonlySpan.LowerBound(value);

            if (lower >= readonlySpan.Length || value.CompareTo(readonlySpan[lower]) != 0)
                return ~lower;

            int upper = readonlySpan[lower..].UpperBound(value) + lower;

            return upper - 1;
        }

        public int LowerBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>
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

        public int UpperBound<TComparable>(TComparable value)
            where TComparable : IComparable<T>
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

        public BinarySearchRange BinarySearchRange<TComparable>(TComparable value)
            where TComparable : IComparable<T>
        {
            int lower = readonlySpan.BinarySearchLowerBound(value);

            if (lower < 0)
                return new(lower, lower);

            int upper = readonlySpan[lower..].BinarySearchUpperBound(value) + lower;

            return new(lower, upper);
        }

        public bool TryBinarySearchRangeSlice<TComparable>(TComparable value, out ReadOnlySpan<T> values)
            where TComparable : IComparable<T>
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

        public ReadOnlySpan<T> BinarySearchRangeSlice<TComparable>(TComparable value)
            where TComparable : IComparable<T> =>
            readonlySpan.TryBinarySearchRangeSlice(value, out var values)
                ? values
                : throw new("not found in slice");
    }

    extension<T>(Span<T> span)
    {
        public bool TryBinarySearchRangeSlice<TComparable>(TComparable value, out Span<T> values)
            where TComparable : IComparable<T>
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

        public Span<T> BinarySearchRangeSlice<TComparable>(TComparable value)
            where TComparable : IComparable<T> =>
            span.TryBinarySearchRangeSlice(value, out var values)
                ? values
                : throw new("not found in slice");
    }
}
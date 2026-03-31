namespace AlirezaMahDev.Extensions.Abstractions;

public static class BinarySearchSpanExtensions
{
    extension<T>(ReadOnlySpan<T> readOnlySpan)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var lower = readOnlySpan.LowerBound(in value);

            return lower < readOnlySpan.Length && value.CompareTo(in readOnlySpan[lower]) == 0
                ? lower
                : ~lower;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound(T value, ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            ScopedReadOnlyComparableComparison<T> scopedReadOnlyComparableComparison = new(value, readOnlyComparison);
            return readOnlySpan.BinarySearchLowerBound(ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound<TComparer>(T value, ref TComparer comparison)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return readOnlySpan.BinarySearchLowerBound(ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct
        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return readOnlySpan.BinarySearchLowerBound(ref scopedReadOnlyComparableComparisonBridge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound<TBridge, TComparer>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ref TComparer comparison)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>
            where TBridge : allows ref struct
        {
            ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer> scopedReadOnlyComparableComparerBridge =
                new(value, func, comparison);
            return readOnlySpan.BinarySearchLowerBound(ref scopedReadOnlyComparableComparerBridge);
        }

        public int BinarySearchUpperBound<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var lower = readOnlySpan.LowerBound(in value);

            if (lower >= readOnlySpan.Length || value.CompareTo(in readOnlySpan[lower]) != 0)
            {
                return ~lower;
            }

            var upper = readOnlySpan[lower..].UpperBound(in value) + lower;

            return upper - 1;
        }

        public int BinarySearchUpperBound(T value, ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            ScopedReadOnlyComparableComparison<T> scopedReadOnlyComparableComparison = new(value, readOnlyComparison);
            return readOnlySpan.BinarySearchUpperBound(ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchUpperBound<TComparer>(T value, ref TComparer comparison)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return readOnlySpan.BinarySearchUpperBound(ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchUpperBound<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return readOnlySpan.BinarySearchUpperBound(ref scopedReadOnlyComparableComparisonBridge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchUpperBound<TBridge, TComparer>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ref TComparer comparison)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>
            where TBridge : allows ref struct
        {
            ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer> scopedReadOnlyComparableComparerBridge =
                new(value, func, comparison);
            return readOnlySpan.BinarySearchUpperBound(ref scopedReadOnlyComparableComparerBridge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int LowerBound<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var lo = 0;
            var hi = readOnlySpan.Length;

            while (lo < hi)
            {
                var mid = lo + ((hi - lo) >> 1);

                if (value.CompareTo(in readOnlySpan[mid]) > 0)
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
        public int LowerBound(T value, ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            ScopedReadOnlyComparableComparison<T> scopedReadOnlyComparableComparison = new(value, readOnlyComparison);
            return readOnlySpan.LowerBound(ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int LowerBound<TComparer>(T value, ref TComparer comparison)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return readOnlySpan.LowerBound(ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int LowerBound<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return readOnlySpan.LowerBound(ref scopedReadOnlyComparableComparisonBridge);
        }

        public int LowerBound<TBridge, TComparer>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ref TComparer comparison)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>

        {
            ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer> scopedReadOnlyComparableComparerBridge =
                new(value, func, comparison);
            return readOnlySpan.LowerBound(ref scopedReadOnlyComparableComparerBridge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var lo = 0;
            var hi = readOnlySpan.Length;

            while (lo < hi)
            {
                var mid = lo + ((hi - lo) >> 1);

                if (value.CompareTo(in readOnlySpan[mid]) >= 0)
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
        public int UpperBound(T value, ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            ScopedReadOnlyComparableComparison<T> scopedReadOnlyComparableComparison = new(value, readOnlyComparison);
            return readOnlySpan.UpperBound(ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound<TComparer>(T value, ref TComparer comparison)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return readOnlySpan.UpperBound(ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return readOnlySpan.UpperBound(ref scopedReadOnlyComparableComparisonBridge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound<TBridge, TComparer>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ref TComparer comparison)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>
            where TBridge : allows ref struct
        {
            ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer> scopedReadOnlyComparableComparerBridge =
                new(value, func, comparison);
            return readOnlySpan.UpperBound(ref scopedReadOnlyComparableComparerBridge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BinarySearchRange BinarySearchRange<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var lower = readOnlySpan.BinarySearchLowerBound(in value);

            if (lower < 0)
            {
                return new(lower, lower);
            }

            var upper = readOnlySpan[lower..].BinarySearchUpperBound(in value);

            if (upper < 0)
            {
                upper = ~upper - 1;
            }

            return new(lower, lower + upper);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BinarySearchRange BinarySearchRange(T value, ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            ScopedReadOnlyComparableComparison<T> scopedReadOnlyComparableComparison = new(value, readOnlyComparison);
            return readOnlySpan.BinarySearchRange(ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BinarySearchRange BinarySearchRange<TComparer>(T value, ref TComparer comparison)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return readOnlySpan.BinarySearchRange(ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BinarySearchRange BinarySearchRange<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return readOnlySpan.BinarySearchRange(ref scopedReadOnlyComparableComparisonBridge);
        }

        public BinarySearchRange BinarySearchRange<TBridge, TComparer>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ref TComparer comparison)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer> scopedReadOnlyComparableComparerBridge =
                new(value, func, comparison);
            return readOnlySpan.BinarySearchRange(ref scopedReadOnlyComparableComparerBridge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TComparable>(scoped ref readonly TComparable value,
            out ReadOnlySpan<T> values)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var binarySearchRange = readOnlySpan.BinarySearchRange(in value);
            if (binarySearchRange.TryGetRange(out var range))
            {
                values = readOnlySpan[range];
                return true;
            }

            values = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice(T value,
            ScopedRefReadOnlyComparison<T> readOnlyComparison,
            out ReadOnlySpan<T> values)
        {
            ScopedReadOnlyComparableComparison<T> scopedReadOnlyComparableComparison = new(value, readOnlyComparison);
            return readOnlySpan.TryBinarySearchRangeSlice(ref scopedReadOnlyComparableComparison, out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TComparer>(T value,
            ref TComparer comparison,
            out ReadOnlySpan<T> values)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return readOnlySpan.TryBinarySearchRangeSlice(ref scopedReadOnlyComparableComparer, out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison,
            out ReadOnlySpan<T> values)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return readOnlySpan.TryBinarySearchRangeSlice(ref scopedReadOnlyComparableComparisonBridge, out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TBridge, TComparer>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ref TComparer comparison,
            out ReadOnlySpan<T> values)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer> scopedReadOnlyComparableComparerBridge =
                new(value, func, comparison);
            return readOnlySpan.TryBinarySearchRangeSlice(ref scopedReadOnlyComparableComparerBridge, out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpan<T> BinarySearchRangeSlice<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            return readOnlySpan.TryBinarySearchRangeSlice(in value, out var values)
                ? values
                : throw new("not found ref slice");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpan<T> BinarySearchRangeSlice(T value, ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            ScopedReadOnlyComparableComparison<T> scopedReadOnlyComparableComparison = new(value, readOnlyComparison);
            return readOnlySpan.BinarySearchRangeSlice(ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpan<T> BinarySearchRangeSlice<TComparer>(T value,
            ref TComparer comparison)
            where TComparer : IComparer<T>, allows ref struct
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return readOnlySpan.BinarySearchRangeSlice(ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpan<T> BinarySearchRangeSlice<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return readOnlySpan.BinarySearchRangeSlice(ref scopedReadOnlyComparableComparisonBridge);
        }

        public ReadOnlySpan<T> BinarySearchRangeSlice<TBridge, TComparer>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ref TComparer comparison)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>, allows ref struct
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer> scopedReadOnlyComparableComparerBridge =
                new(value, func, comparison);
            return readOnlySpan.BinarySearchRangeSlice(ref scopedReadOnlyComparableComparerBridge);
        }
    }
}
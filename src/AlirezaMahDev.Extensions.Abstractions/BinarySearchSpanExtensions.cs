namespace AlirezaMahDev.Extensions.Abstractions;

public static class BinarySearchSpanExtensions
{
    extension<TSelf, T>(TSelf refReadOnlyArray)
        where TSelf : IRefReadOnlyBlock<TSelf, T, RefReadOnlyEnumerator<TSelf, T>>, allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var lower = refReadOnlyArray.LowerBound<TSelf, T, TComparable>(in value);

            return lower < refReadOnlyArray.Length && value.CompareTo(in refReadOnlyArray[lower]) == 0
                ? lower
                : ~lower;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound(T value, ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            ScopedReadOnlyComparableComparison<T> scopedReadOnlyComparableComparison = new(value, readOnlyComparison);
            return refReadOnlyArray.BinarySearchLowerBound<TSelf, T, ScopedReadOnlyComparableComparison<T>>(
                ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound<TComparer>(T value, ref TComparer comparison)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return refReadOnlyArray.BinarySearchLowerBound<TSelf, T, ScopedReadOnlyComparableComparer<T, TComparer>>(
                ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchLowerBound<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct
        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return refReadOnlyArray
                .BinarySearchLowerBound<TSelf, T, ScopedReadOnlyComparableComparisonBridge<T, TBridge>>(
                    ref scopedReadOnlyComparableComparisonBridge);
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
            return refReadOnlyArray
                .BinarySearchLowerBound<TSelf, T, ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer>>(
                    ref scopedReadOnlyComparableComparerBridge);
        }

        public int BinarySearchUpperBound<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var lower = refReadOnlyArray.LowerBound<TSelf, T, TComparable>(in value);

            if (lower >= refReadOnlyArray.Length || value.CompareTo(in refReadOnlyArray[lower]) != 0)
            {
                return ~lower;
            }

            var upper = refReadOnlyArray[lower..].UpperBound<TSelf, T, TComparable>(in value) + lower;

            return upper - 1;
        }

        public int BinarySearchUpperBound(T value, ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            ScopedReadOnlyComparableComparison<T> scopedReadOnlyComparableComparison = new(value, readOnlyComparison);
            return refReadOnlyArray.BinarySearchUpperBound<TSelf, T, ScopedReadOnlyComparableComparison<T>>(
                ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchUpperBound<TComparer>(T value, ref TComparer comparison)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return refReadOnlyArray.BinarySearchUpperBound<TSelf, T, ScopedReadOnlyComparableComparer<T, TComparer>>(
                ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int BinarySearchUpperBound<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return refReadOnlyArray
                .BinarySearchUpperBound<TSelf, T, ScopedReadOnlyComparableComparisonBridge<T, TBridge>>(
                    ref scopedReadOnlyComparableComparisonBridge);
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
            return refReadOnlyArray
                .BinarySearchUpperBound<TSelf, T, ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer>>(
                    ref scopedReadOnlyComparableComparerBridge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int LowerBound<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var lo = 0;
            var hi = refReadOnlyArray.Length;

            while (lo < hi)
            {
                var mid = lo + ((hi - lo) >> 1);

                if (value.CompareTo(in refReadOnlyArray[mid]) > 0)
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
            return refReadOnlyArray.LowerBound<TSelf, T, ScopedReadOnlyComparableComparison<T>>(
                ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int LowerBound<TComparer>(T value, ref TComparer comparison)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return refReadOnlyArray.LowerBound<TSelf, T, ScopedReadOnlyComparableComparer<T, TComparer>>(
                ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int LowerBound<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return refReadOnlyArray.LowerBound<TSelf, T, ScopedReadOnlyComparableComparisonBridge<T, TBridge>>(
                ref scopedReadOnlyComparableComparisonBridge);
        }

        public int LowerBound<TBridge, TComparer>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ref TComparer comparison)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>

        {
            ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer> scopedReadOnlyComparableComparerBridge =
                new(value, func, comparison);
            return refReadOnlyArray.LowerBound<TSelf, T, ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer>>(
                ref scopedReadOnlyComparableComparerBridge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var lo = 0;
            var hi = refReadOnlyArray.Length;

            while (lo < hi)
            {
                var mid = lo + ((hi - lo) >> 1);

                if (value.CompareTo(in refReadOnlyArray[mid]) >= 0)
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
            return refReadOnlyArray.UpperBound<TSelf, T, ScopedReadOnlyComparableComparison<T>>(
                ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound<TComparer>(T value, ref TComparer comparison)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return refReadOnlyArray.UpperBound<TSelf, T, ScopedReadOnlyComparableComparer<T, TComparer>>(
                ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int UpperBound<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return refReadOnlyArray.UpperBound<TSelf, T, ScopedReadOnlyComparableComparisonBridge<T, TBridge>>(
                ref scopedReadOnlyComparableComparisonBridge);
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
            return refReadOnlyArray.UpperBound<TSelf, T, ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer>>(
                ref scopedReadOnlyComparableComparerBridge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BinarySearchRange BinarySearchRange<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var lower = refReadOnlyArray.BinarySearchLowerBound<TSelf, T, TComparable>(in value);

            if (lower < 0)
            {
                return new(lower, lower);
            }

            var upper = refReadOnlyArray[lower..].BinarySearchUpperBound<TSelf, T, TComparable>(in value);

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
            return refReadOnlyArray.BinarySearchRange<TSelf, T, ScopedReadOnlyComparableComparison<T>>(
                ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BinarySearchRange BinarySearchRange<TComparer>(T value, ref TComparer comparison)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return refReadOnlyArray.BinarySearchRange<TSelf, T, ScopedReadOnlyComparableComparer<T, TComparer>>(
                ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BinarySearchRange BinarySearchRange<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return refReadOnlyArray.BinarySearchRange<TSelf, T, ScopedReadOnlyComparableComparisonBridge<T, TBridge>>(
                ref scopedReadOnlyComparableComparisonBridge);
        }

        public BinarySearchRange BinarySearchRange<TBridge, TComparer>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ref TComparer comparison)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer> scopedReadOnlyComparableComparerBridge =
                new(value, func, comparison);
            return refReadOnlyArray
                .BinarySearchRange<TSelf, T, ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer>>(
                    ref scopedReadOnlyComparableComparerBridge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TComparable>(scoped ref readonly TComparable value,
            out TSelf values)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            var binarySearchRange = refReadOnlyArray.BinarySearchRange<TSelf, T, TComparable>(in value);
            if (binarySearchRange.TryGetRange(out var range))
            {
                values = refReadOnlyArray[range];
                return true;
            }

            values = default!;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice(T value,
            ScopedRefReadOnlyComparison<T> readOnlyComparison,
            out TSelf values)
        {
            ScopedReadOnlyComparableComparison<T> scopedReadOnlyComparableComparison = new(value, readOnlyComparison);
            return refReadOnlyArray.TryBinarySearchRangeSlice<TSelf, T, ScopedReadOnlyComparableComparison<T>>(
                ref scopedReadOnlyComparableComparison,
                out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TComparer>(T value,
            ref TComparer comparison,
            out TSelf values)
            where TComparer : IComparer<T>
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return refReadOnlyArray.TryBinarySearchRangeSlice<TSelf, T, ScopedReadOnlyComparableComparer<T, TComparer>>(
                ref scopedReadOnlyComparableComparer,
                out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison,
            out TSelf values)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return refReadOnlyArray
                .TryBinarySearchRangeSlice<TSelf, T, ScopedReadOnlyComparableComparisonBridge<T, TBridge>>(
                    ref scopedReadOnlyComparableComparisonBridge,
                    out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool TryBinarySearchRangeSlice<TBridge, TComparer>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ref TComparer comparison,
            out TSelf values)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer> scopedReadOnlyComparableComparerBridge =
                new(value, func, comparison);
            return refReadOnlyArray
                .TryBinarySearchRangeSlice<TSelf, T, ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer>>(
                    ref scopedReadOnlyComparableComparerBridge,
                    out values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TSelf BinarySearchRangeSlice<TComparable>(scoped ref readonly TComparable value)
            where TComparable : IScopedRefReadOnlyComparable<T>, allows ref struct
        {
            return refReadOnlyArray.TryBinarySearchRangeSlice<TSelf, T, TComparable>(in value, out var values)
                ? values
                : throw new("not found ref slice");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TSelf BinarySearchRangeSlice(T value, ScopedRefReadOnlyComparison<T> readOnlyComparison)
        {
            ScopedReadOnlyComparableComparison<T> scopedReadOnlyComparableComparison = new(value, readOnlyComparison);
            return refReadOnlyArray.BinarySearchRangeSlice<TSelf, T, ScopedReadOnlyComparableComparison<T>>(
                ref scopedReadOnlyComparableComparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TSelf BinarySearchRangeSlice<TComparer>(T value,
            ref TComparer comparison)
            where TComparer : IComparer<T>, allows ref struct
        {
            ScopedReadOnlyComparableComparer<T, TComparer> scopedReadOnlyComparableComparer = new(value, comparison);
            return refReadOnlyArray.BinarySearchRangeSlice<TSelf, T, ScopedReadOnlyComparableComparer<T, TComparer>>(
                ref scopedReadOnlyComparableComparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TSelf BinarySearchRangeSlice<TBridge>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparisonBridge<T, TBridge> scopedReadOnlyComparableComparisonBridge =
                new(value, func, readOnlyComparison);
            return refReadOnlyArray
                .BinarySearchRangeSlice<TSelf, T, ScopedReadOnlyComparableComparisonBridge<T, TBridge>>(
                    ref scopedReadOnlyComparableComparisonBridge);
        }

        public TSelf BinarySearchRangeSlice<TBridge, TComparer>(ref TBridge value,
            ScopedRefReadOnlyFunc<T, TBridge> func,
            ref TComparer comparison)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>, allows ref struct
            where TBridge : allows ref struct

        {
            ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer> scopedReadOnlyComparableComparerBridge =
                new(value, func, comparison);
            return refReadOnlyArray
                .BinarySearchRangeSlice<TSelf, T, ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer>>(
                    ref scopedReadOnlyComparableComparerBridge);
        }
    }
}
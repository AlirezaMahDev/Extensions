namespace AlirezaMahDev.Extensions.Abstractions;

public static class SpanSortExtensions
{
    extension<T>(Span<T> span)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Sort<TBridge>(ScopedRefReadOnlyFunc<T, TBridge> func,
            ScopedRefReadOnlyComparison<TBridge> readOnlyComparison)
            where TBridge : allows ref struct
        {
            span.Sort(new ScopedComparisonBridge<T, TBridge>(func, readOnlyComparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Sort<TBridge, TComparer>(ScopedRefReadOnlyFunc<T, TBridge> func, TComparer comparison)
            where TComparer : IScopedRefReadOnlyComparer<TBridge>
            where TBridge : allows ref struct
        {
            span.Sort(new ScopedComparerBridge<T, TBridge, TComparer>(func, comparison));
        }
    }
}
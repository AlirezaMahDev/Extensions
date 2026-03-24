namespace AlirezaMahDev.Extensions.Abstractions;

public static class SpanSortExtensions
{
    extension<T>(Span<T> span)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Sort<TBridge>(Func<T, TBridge> func,
            Comparison<TBridge> comparison)
            where TBridge : allows ref struct
        {
            span.Sort(new ComparisonBridge<T, TBridge>(func, comparison));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Sort<TBridge, TComparer>(Func<T, TBridge> func, TComparer comparison)
            where TComparer : IComparer<TBridge>
            where TBridge : allows ref struct
        {
            span.Sort(new ComparerBridge<T, TBridge, TComparer>(func, comparison));
        }
    }
}
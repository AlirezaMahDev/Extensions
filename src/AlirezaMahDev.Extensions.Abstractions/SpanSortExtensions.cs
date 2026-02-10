namespace AlirezaMahDev.Extensions.Abstractions;

public static class SpanSortExtensions
{
    extension<T>(Span<T> span)
    {
        public void Sort<TBridge>(Func<T, TBridge> func,
            Comparison<TBridge> comparison) =>
            span.Sort(new ComparisonBridge<T, TBridge>(func, comparison));

        public void Sort<TBridge, TComparer>(Func<T, TBridge> func, TComparer comparison)
            where TComparer : IComparer<TBridge> =>
            span.Sort(new ComparerBridge<T, TBridge, TComparer>(func, comparison));
    }
}
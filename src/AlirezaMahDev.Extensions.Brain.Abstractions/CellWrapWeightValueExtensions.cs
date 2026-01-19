namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CellWrapWeightValueExtensions
{
    extension<TCellWrap, TValue, TCell>(TCellWrap wrap)
        where TCellWrap : ICellWrap<TCell, TValue>
        where TValue : unmanaged, ICellValue<TValue>, ICellWeightValue
        where TCell : ICell
    {
        public static Comparison<TCellWrap> ComparerOnWeight() =>
            (a, b) => b.RefValue.RefWeight.CompareTo(a.RefValue.RefWeight);
    }
}
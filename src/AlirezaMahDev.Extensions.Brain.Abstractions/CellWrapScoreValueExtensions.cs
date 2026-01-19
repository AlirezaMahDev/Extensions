namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CellWrapScoreValueExtensions
{
    extension<TCellWrap, TValue, TCell>(TCellWrap wrap)
        where TCellWrap : ICellWrap<TCell, TValue>
        where TValue : unmanaged, ICellValue<TValue>, ICellScoreValue
        where TCell : ICell
    {
        public static Comparison<TCellWrap> ComparerOnScore() =>
            (a, b) => b.RefValue.RefScore.CompareTo(a.RefValue.RefScore);
    }
}
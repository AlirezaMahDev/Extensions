namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CellWrapScoreValueExtensions
{
    extension<TCell, TValue, TData, TLink>(CellWrap<TCell, TValue, TData, TLink>)
        where TCell : ICell
        where TValue : unmanaged, ICellValue<TValue>, ICellScoreValue
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public static Comparison<CellWrap<TCell, TValue, TData, TLink>> ComparerOnScore() =>
            (a, b) => b.RefValue.RefScore.CompareTo(a.RefValue.RefScore);
    }
}
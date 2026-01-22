namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CellWrapWeightValueExtensions
{
    extension<TCell, TValue, TData, TLink>(CellWrap<TCell, TValue, TData, TLink>)
        where TCell : ICell
        where TValue : unmanaged, ICellValue<TValue>, ICellWeightValue
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public static Comparison<CellWrap<TCell, TValue, TData, TLink>> ComparerOnWeight() =>
            (a, b) => b.RefValue.RefWeight.CompareTo(a.RefValue.RefWeight);
    }
}
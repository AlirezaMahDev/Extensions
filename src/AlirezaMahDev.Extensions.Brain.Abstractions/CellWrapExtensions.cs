namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CellWrapExtensions
{
    extension<TCell, TValue, TData, TLink>(CellWrap<TCell, TValue, TData, TLink> cellWrap)
        where TCell : ICell
        where TValue : unmanaged, ICellValue<TValue>
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
    }
}
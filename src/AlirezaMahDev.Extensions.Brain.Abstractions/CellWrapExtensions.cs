namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CellWrapExtensions
{
    // ReSharper disable once EmptyExtensionBlock
    extension<TValue, TData, TLink>(in CellWrap<TValue, TData, TLink> cellWrap)
        where TValue : unmanaged, ICellValue<TValue>
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
    }
}
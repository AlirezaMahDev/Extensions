namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellWrap<out TCell, TValue>
    where TCell : ICell
    where TValue : unmanaged, ICellValue<TValue>
{
    TCell Cell { get; }

    // DataWrap<TValue> Location { get; }
    ref readonly TValue RefValue { get; }
}

public interface ICellWrap<out TCell, TValue, TData, TLink> : ICellWrap<TCell, TValue>
    where TCell : ICell
    where TValue : unmanaged, ICellValue<TValue>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    INerve<TData, TLink> Nerve { get; }
}
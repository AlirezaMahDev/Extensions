using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellWrap<out TCell, TValue>
    where TCell : ICell
    where TValue : unmanaged, ICellValue<TValue>
{
    TCell Cell { get; }
    DataWrap<TValue> Location { get; }
    ref readonly TValue RefValue { get; }
}
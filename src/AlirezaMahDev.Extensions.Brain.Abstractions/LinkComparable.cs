using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly record struct LinkComparable<TData, TLink>(
    TLink Link,
    Comparison<TLink> Comparison)
    : IComparable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public int CompareTo(CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> other) =>
        Comparison(Link, other.RefLink);
}

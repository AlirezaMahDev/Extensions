using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly record struct ThinkValueComparable<TData, TLink>(
    ThinkValue<TData, TLink> DataPairLink,
    Comparison<ThinkValue<TData, TLink>> Comparison)
    : IComparable<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public int CompareTo(CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> other) =>
        Comparison(DataPairLink,
            new(other.NeuronWrap.RefData, other.RefLink, other.RefValue.RefScore, other.RefValue.RefWeight));
}
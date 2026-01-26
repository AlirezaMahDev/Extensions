using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly record struct DataPairLink<TData, TLink>(TData Data, TLink Link)
    : IComparable<DataPairLink<TData, TLink>>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public int CompareTo(DataPairLink<TData, TLink> other)
    {
        var link = Link.CompareTo(other.Link);
        if (link != 0)
            return link;

        var data = Data.CompareTo(other.Data);
        if (data != 0)
            return data;

        return 0;
    }
}
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly ref struct ThinkValueRef<TData, TLink>(
    in TData data,
    in TLink link,
    in float score = float.MaxValue,
    in uint weight = uint.MaxValue)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public readonly ref readonly TData Data = ref data;
    public readonly ref readonly TLink Link = ref link;
    public readonly ref readonly float Score = ref score;
    public readonly ref readonly uint Weight = ref weight;
}
namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly struct ThinkValue<TData, TLink>(
    TData data,
    TLink link,
    float score = float.MaxValue,
    uint weight = uint.MaxValue)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public readonly TData Data = data;
    public readonly TLink Link = link;
    public readonly float Score = score;
    public readonly uint Weight = weight;
}
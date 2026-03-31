namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly ref struct ThinkValueRef<TData, TLink>(
    ref readonly TData data,
    ref readonly TLink link,
    float score = float.MaxValue,
    uint weight = uint.MaxValue)
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public readonly ref readonly TData Data = ref data;
    public readonly ref readonly TLink Link = ref link;
    public readonly float Score = score;
    public readonly uint Weight = weight;
}
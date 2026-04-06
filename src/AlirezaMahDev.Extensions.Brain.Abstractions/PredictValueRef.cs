namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly ref struct PredictValueRef<TLink>(
    TLink link,
    float score = float.MaxValue,
    uint weight = uint.MaxValue)
    where TLink : unmanaged, ICellLink<TLink>
{
    public readonly TLink Link = link;
    public readonly float Score = score;
    public readonly uint Weight = weight;
}
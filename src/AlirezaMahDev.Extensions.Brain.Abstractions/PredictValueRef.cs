using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly ref struct PredictValueRef<TLink>(
    in TLink link,
    in float score = float.MaxValue,
    in uint weight = uint.MaxValue)
    where TLink : unmanaged, ICellLink<TLink>
{
    public readonly ref readonly TLink Link = ref link;
    public readonly ref readonly float Score = ref score;
    public readonly ref readonly uint Weight = ref weight;
}
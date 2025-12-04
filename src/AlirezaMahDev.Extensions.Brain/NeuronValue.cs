using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct NeuronValue<TData>
{
    public int Connection;
    public TData Data;
    public float Score;
    public uint Weight;
}
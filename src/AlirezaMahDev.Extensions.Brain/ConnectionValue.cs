using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct ConnectionValue
{
    public int Neuron;
    public int Next;
    public int Previous;
    public float Score;
    public uint Weight;
}
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct ConnectionValue(long Next) :
    IDataValueDefault<ConnectionValue>,
    IDataValue<ConnectionValue>
{
    public long Neuron;
    public long Previous;
    public float Score;
    public uint Weight;

    public static ConnectionValue Default { get; } = new()
    {
        Neuron = -1L,
        Previous = -1L,
        Score = 1f,
        Weight = 0u,
        Next = -1L,
    };
}
using System.Numerics;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct ConnectionValue<TLink> :
    IDataValueDefault<ConnectionValue<TLink>>,
    IDataValue<ConnectionValue<TLink>>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public long Neuron;
    public long SubConnection;
    public long NextSubConnection;
    public long Next;
    public long Previous;
    public float Score;
    public uint Weight;
    public TLink Link;

    public static ConnectionValue<TLink> Default { get; } = new()
    {
        Neuron = -1L,
        SubConnection = -1L,
        NextSubConnection = -1L,
        Next = -1L,
        Previous = -1L,
        Score = 1f,
        Weight = 0u,
        Link = default
    };
}
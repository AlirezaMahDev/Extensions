using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct ConnectionValue<TLink> :
    IDataValueDefault<ConnectionValue<TLink>>,
    IDataLock<ConnectionValue<TLink>>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public DataOffset Neuron;
    public DataOffset Child;
    public DataOffset Next;
    public DataOffset Previous;
    public float Score;
    public uint Weight;
    public TLink Link;

    public static ConnectionValue<TLink> Default { get; } = new()
    {
        Neuron = DataOffset.Null,
        Next = DataOffset.Null,
        Child = DataOffset.Null,
        Previous = DataOffset.Null,
        Score = 1f,
        Weight = 0u,
        Link = default
    };
    
    public int RefLock;
    public ref int Lock => ref Unsafe.AsRef(in this).RefLock;
}
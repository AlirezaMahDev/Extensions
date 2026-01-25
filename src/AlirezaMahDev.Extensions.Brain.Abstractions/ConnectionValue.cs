using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct ConnectionValue<TLink> :
    ICellValueDefault<ConnectionValue<TLink>>,
    ICellScoreValue,
    ICellWeightValue
    where TLink : unmanaged, ICellLink<TLink>
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
    public readonly ref int Lock => ref Unsafe.AsRef(in this).RefLock;

    public readonly ref float RefScore => ref Unsafe.AsRef(in this).Score;
    public readonly ref uint RefWeight => ref Unsafe.AsRef(in this).Weight;
}
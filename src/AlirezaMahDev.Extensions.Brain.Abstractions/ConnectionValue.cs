using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct ConnectionValue<TLink> :
    ICellValueDefault<ConnectionValue<TLink>>,
    ICellScoreValue,
    ICellWeightValue
    where TLink : unmanaged, ICellLink<TLink>
{
    public TLink Link;

    public DataOffset Neuron;
    public DataOffset Child;
    public DataOffset Next;
    public DataOffset Previous;

    public int NextCount;
    public uint Weight;
    public float Score;

    public static ConnectionValue<TLink> Default { get; } = new()
    {
        Neuron = DataOffset.Null,
        Next = DataOffset.Null,
        NextCount = 0,
        Child = DataOffset.Null,
        Previous = DataOffset.Null,
        Score = 1f,
        Weight = 0u,
        Link = default
    };

    public int Lock;
    public readonly ref int RefLock => ref Unsafe.AsRef(in this).Lock;

    public readonly ref float RefScore => ref Unsafe.AsRef(in this).Score;
    public readonly ref uint RefWeight => ref Unsafe.AsRef(in this).Weight;
}
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public record struct NeuronValue<TData> :
    ICellValueDefault<NeuronValue<TData>>,
    ICellScoreValue,
    ICellWeightValue
    where TData : unmanaged, ICellData<TData>
{
    public TData Data;

    public DataOffset Connection;

    public uint Weight;
    public float Score;

    public static NeuronValue<TData> Default { get; } = new()
    {
        Connection = DataOffset.Null,
        Data = default,
        Score = 1f,
        Weight = 0u
    };

    public int Lock;

    public readonly ref int RefLock => ref Unsafe.AsRef(in this).Lock;
    public readonly ref float RefScore => ref Unsafe.AsRef(in this).Score;
    public readonly ref uint RefWeight => ref Unsafe.AsRef(in this).Weight;
}
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct NeuronValue<TData> :
    ICellValueDefault<NeuronValue<TData>>,
    ICellScoreValue, ICellWeightValue
    where TData : unmanaged, ICellData<TData>
{
    public TData Data;
    public float Score;
    public uint Weight;
    public DataOffset Connection;

    public static NeuronValue<TData> Default { get; } = new()
    {
        Connection = DataOffset.Null,
        Data = default,
        Score = 1f,
        Weight = 0u
    };

    public int RefLock;

    public readonly ref int Lock => ref Unsafe.AsRef(in this).RefLock;
    public readonly ref float RefScore => ref Unsafe.AsRef(in this).Score;
    public readonly ref uint RefWeight => ref Unsafe.AsRef(in this).Weight;
}
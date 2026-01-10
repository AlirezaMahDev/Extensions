using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct NeuronValue<TData> :
    IDataValueDefault<NeuronValue<TData>>,
    IDataLock<NeuronValue<TData>>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
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
    public ref int Lock => ref Unsafe.AsRef(in this).RefLock;
}
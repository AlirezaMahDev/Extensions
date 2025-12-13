using System.Numerics;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct NeuronValue<TData> :
    IDataValueDefault<NeuronValue<TData>>,
    IDataValue<NeuronValue<TData>>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
{
    public TData Data;
    public float Score;
    public uint Weight;
    public long Connection;

    public static NeuronValue<TData> Default { get; } = new()
    {
        Connection = -1L,
        Data = default,
        Score = 1f,
        Weight = 0u
    };
}
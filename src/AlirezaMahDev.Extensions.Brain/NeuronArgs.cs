using System.Numerics;
using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
readonly record struct NeuronArgs<TData, TLink>(Nerve<TData, TLink> Nerve, DataLocation<NeuronValue<TData>> Location)
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public Nerve<TData, TLink> Nerve { get; } = Nerve;
}

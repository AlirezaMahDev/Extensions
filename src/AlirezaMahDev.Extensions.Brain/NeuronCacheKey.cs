using System.Numerics;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
record struct NeuronCacheKey<TData, TLink>(TData Data, TLink Link, long Connection)
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>;
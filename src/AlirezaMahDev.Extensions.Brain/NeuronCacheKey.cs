using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.Brain.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
record struct NeuronCacheKey<TData, TLink>(TData Data, TLink Link, IConnection<TData, TLink>? Connection)
    where TData : unmanaged
    where TLink : unmanaged;
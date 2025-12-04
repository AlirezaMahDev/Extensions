using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct NeuronCacheKey<TData>(TData Data, Connection<TData>? Connection)
    where TData : unmanaged;
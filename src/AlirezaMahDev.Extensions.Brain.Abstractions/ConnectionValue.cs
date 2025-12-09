using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct ConnectionValue<TLink>(long Next) :
    IDataValueDefault<ConnectionValue<TLink>>,
    IDataValue<ConnectionValue<TLink>>
    where TLink : unmanaged
{
    public long Neuron;
    public long Previous;
    public float Score;
    public uint Weight;
    public TLink Link;

    public static ConnectionValue<TLink> Default { get; } = new()
    {
        Neuron = -1L,
        Previous = -1L,
        Score = 1f,
        Weight = 0u,
        Next = -1L,
    };
}

public record struct NearConnection<TData, TLink>(
    TData Data,
    TLink Link,
    IConnection<TData, TLink> Connection)
    where TData : unmanaged
    where TLink : unmanaged;

public record struct ThinkResult<TData, TLink>(NearConnection<TData, TLink>[] Connections, NearConnection<TData, TLink>? Next)
    where TData : unmanaged
    where TLink : unmanaged;
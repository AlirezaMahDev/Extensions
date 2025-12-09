using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface IConnection<TData, TLink> : IEnumerable<IConnection<TData, TLink>>
    where TData : unmanaged
    where TLink : unmanaged
{
    DataLocation<ConnectionValue<TLink>> Location { get; }
    long Offset { get; }
    ref ConnectionValue<TLink> RefValue { get; }
    ref TLink RefLink { get; }
    INeuron<TData, TLink> Neuron { get; }
    IConnection<TData, TLink>? Previous { get; }
    IConnection<TData, TLink>? Next { get; }
    IConnection<TData, TLink>[] ToArray();
}
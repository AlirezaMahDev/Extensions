using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface IConnection<TData> : IEnumerable<IConnection<TData>>
    where TData : unmanaged
{
    DataLocation<ConnectionValue> Location { get; }
    long Offset { get; }
    ref ConnectionValue RefValue { get; }
    INeuron<TData> Neuron { get; }
    IConnection<TData>? Previous { get; }
    IConnection<TData>? Next { get; }
    IConnection<TData>[] ToArray();
}
namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INeuron<TData> : IEnumerable<IConnection<TData>>
    where TData : unmanaged
{
    long Offset { get; }
    ref NeuronValue<TData> RefValue { get; }
    ref TData RefData { get; }
    
    IConnection<TData>? Connection { get; }
    IConnection<TData> GetOrAdd(TData data, IConnection<TData>? connection);
}
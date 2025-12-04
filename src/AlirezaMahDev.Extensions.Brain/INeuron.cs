namespace AlirezaMahDev.Extensions.Brain;

public interface INeuron<TData> : IEnumerable<Connection<TData>>
    where TData : unmanaged
{
    int Id { get; }
    ref NeuronValue<TData> RefValue { get; }
    ref TData RefData { get; }
    Connection<TData>? Connection { get; }
    Connection<TData> GetOrAdd(TData data, Connection<TData>? connection);
}
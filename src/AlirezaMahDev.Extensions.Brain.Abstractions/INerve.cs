namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerve<TData, TLink>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    INerveCache Cache { get; }

    ConcurrentDictionary<DataOffset, Lazy<CellMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>>
        MemoryCache
    { get; }

    IDataAccess Access { get; }
    string Name { get; }

    ref readonly DataLocation<DataPath> Location { get; }
    ref readonly DataLocation<DataPath> ConnectionLocation { get; }
    ref readonly DataLocation<DataPath> NeuronLocation { get; }
    ref readonly DataLocation<DataPath> CounterLocation { get; }

    void Flush();

    ref readonly Neuron Neuron { get; }
    ref readonly CellWrap<Neuron, NeuronValue<TData>, TData, TLink> RootNeuronWrap { get; }
    ref readonly Connection Connection { get; }
    ref readonly CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> RootConnectionWrap { get; }
    ref readonly DataWrap<NerveCounter> Counter { get; }
}
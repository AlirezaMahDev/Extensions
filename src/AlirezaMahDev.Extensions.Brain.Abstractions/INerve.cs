using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.Caching.Memory;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerve<TData, TLink>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    INerveCache Cache { get; }
    IMemoryCache MemoryCache { get; }

    IDataAccess Access { get; }
    string Name { get; }

    DataLocation<DataPath> Location { get; }
    DataLocation<DataPath> ConnectionLocation { get; }
    DataLocation<DataPath> NeuronLocation { get; }
    DataLocation<DataPath> CounterLocation { get; }

    void Flush();

    Neuron Neuron { get; }
    CellWrap<Neuron, NeuronValue<TData>, TData, TLink> NeuronWrap { get; }
    Connection Connection { get; }
    CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> ConnectionWrap { get; }
    DataWrap<NerveCounter> Counter { get; }
}
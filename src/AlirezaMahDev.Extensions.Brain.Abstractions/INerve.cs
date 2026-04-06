namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerve<TData, TLink>
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    INerveCache Cache { get; }

    ConcurrentDictionary<DataOffset, Lazy<CellMemory<CellWrap<ConnectionValue<TLink>, TData, TLink>>>>
        MemoryCache
    {
        get;
    }

    IDataAccess Access { get; }
    string Name { get; }

    ref readonly DataLocation<DataPath> Location { get; }
    ref readonly DataLocation<DataPath> ConnectionLocation { get; }
    ref readonly DataLocation<DataPath> NeuronLocation { get; }

    void Flush();

    ref readonly Neuron RootNeuron { get; }
    ref readonly CellWrap<NeuronValue<TData>, TData, TLink> RootNeuronWrap { get; }
    ref readonly Connection RootConnection { get; }
    ref readonly CellWrap<ConnectionValue<TLink>, TData, TLink> RootConnectionWrap { get; }
}
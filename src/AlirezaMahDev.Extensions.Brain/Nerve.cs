using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class Nerve<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    TData,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    TLink> : INerve<TData, TLink>, IDisposable
    where TData : unmanaged, ICellData<TData>
    where TLink : unmanaged, ICellLink<TLink>
{
    public ConcurrentDictionary<DataOffset,
        Lazy<CellMemory<CellWrap<Connection, ConnectionValue<TLink>, TData, TLink>>>> MemoryCache { get; }

    public IDataAccess Access { get; }
    public INerveCache Cache { get; }

    public string Name { get; }

    public DataLocation<DataPath> Location { get; }
    public DataLocation<DataPath> ConnectionLocation { get; }
    public DataLocation<DataPath> NeuronLocation { get; }

    public Neuron Neuron { get; }
    public CellWrap<Neuron, NeuronValue<TData>, TData, TLink> NeuronWrap { get; }

    public Connection Connection { get; }
    public CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> ConnectionWrap { get; }

    public DataLocation<DataPath> CounterLocation { get; }

    public DataWrap<NerveCounter> Counter { get; }

    public Nerve(IDataManager dataManager, string name)
    {
        Cache = new NerveCache();

        MemoryCache = new();
        Name = name;
        Access = Name.StartsWith("temp:") ? dataManager.OpenTemp() : dataManager.Open(name);
        Location = Access.Root.Wrap(Access, x => x.Dictionary()).GetOrAdd(".nerve");
        ConnectionLocation = Location.Wrap(Access, x => x.Dictionary()).GetOrAdd(".connection");
        NeuronLocation = Location.Wrap(Access, x => x.Dictionary()).GetOrAdd(".neuron");
        CounterLocation = Location.Wrap(Access, x => x.Dictionary()).GetOrAdd(".counter");

        var neuron = NeuronLocation
            .Wrap(Access, x => x.Storage())
            .GetOrCreateData(NeuronValue<TData>.Default);
        var connection = ConnectionLocation
            .Wrap(Access, x => x.Storage())
            .GetOrCreateData(ConnectionValue<TLink>.Default with { Neuron = neuron.Offset });
        Counter = CounterLocation
            .Wrap(Access, x => x.Storage())
            .GetOrCreateData(NerveCounter.Default)
            .Wrap(Access);

        Neuron = new(neuron.Offset);
        NeuronWrap = Neuron.Wrap(this);
        Connection = new(connection.Offset);
        ConnectionWrap = Connection.Wrap(this);
    }

    public void Flush()
    {
        Access.Flush();
    }

    public void Dispose()
    {
        this.CleanThink();
    }
}
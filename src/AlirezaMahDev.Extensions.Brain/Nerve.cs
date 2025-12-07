using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlirezaMahDev.Extensions.Brain;

class Nerve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TData> : INerve<TData>
    where TData : unmanaged
{
    private readonly IDataAccess _dataAccess;
    private readonly ILogger<Nerve<TData>> _logger;
    // private readonly ConcurrentDictionary<int, Lazy<RootConnection<TData>>> _rootLevelConnections = [];

    private readonly ConcurrentDictionary<TData, long> _neuronCache = [];

    public string Name { get; }
    public DataLocation<DataPath> Location { get; }

    public NeuronFactory<TData> NeuronFactory { get; }
    public ConnectionFactory<TData> ConnectionFactory { get; }

    public IConnection<TData> Root { get; }

    public Nerve(IServiceProvider serviceProvider, IDataManager dataManager, string name, ILogger<Nerve<TData>> logger)
    {
        _logger = logger;
        Name = name;
        _dataAccess = dataManager.Open(name);
        Location = _dataAccess.GetRoot().Wrap(x => x.Dictionary()).GetOrAdd(".nerve");

        NeuronFactory = ActivatorUtilities.CreateInstance<NeuronFactory<TData>>(serviceProvider, this);
        ConnectionFactory = ActivatorUtilities.CreateInstance<ConnectionFactory<TData>>(serviceProvider, this);
        var neuron = NeuronFactory.Location
            .Wrap(x => x.Storage())
            .GetOrCreateData(NeuronValue<TData>.Default);
        var connection = ConnectionFactory.Location
            .Wrap(x => x.Storage())
            .GetOrCreateData(new ConnectionValue() { Next = neuron.Offset });
        Root = ConnectionFactory.GetOrCreate(connection.Offset);
    }

    public void Learn(params ReadOnlySpan<TData> data)
    {
        using var enumerator = data.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return;
        }

        var connection = Root.Neuron.GetOrAdd(enumerator.Current, null);
        Interlocked.Increment(ref connection.Neuron.RefValue.Weight);
        Interlocked.Increment(ref connection.RefValue.Weight);
        while (enumerator.MoveNext())
        {
            connection = connection.Neuron.GetOrAdd(enumerator.Current, connection);
            Interlocked.Increment(ref connection.Neuron.RefValue.Weight);
            Interlocked.Increment(ref connection.RefValue.Weight);
        }
    }

    public void Sleep()
    {
        throw new NotImplementedException();
    }

    public TData? Think(params ReadOnlySpan<TData> data)
    {
        throw new NotImplementedException();
    }

    public void Save()
    {
        _dataAccess.Save();
    }

    public async ValueTask SaveAsync()
    {
        await _dataAccess.SaveAsync();
    }
}
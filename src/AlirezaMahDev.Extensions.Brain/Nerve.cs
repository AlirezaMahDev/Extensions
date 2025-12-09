using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlirezaMahDev.Extensions.Brain;

class Nerve<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    TData,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    TLink> : INerve<TData, TLink>
    where TData : unmanaged
    where TLink : unmanaged
{
    private readonly IDataAccess _dataAccess;

    private readonly ILogger<Nerve<TData, TLink>> _logger;

    // private readonly ConcurrentDictionary<int, Lazy<RootConnection<TData>>> _rootLevelConnections = [];
    private readonly ConcurrentDictionary<TData, long> _neuronCache = [];

    public string Name { get; }
    public DataLocation<DataPath> Location { get; }

    public NeuronFactory<TData, TLink> NeuronFactory { get; }
    public ConnectionFactory<TData, TLink> ConnectionFactory { get; }

    public IConnection<TData, TLink> Root { get; }

    public Nerve(IServiceProvider serviceProvider,
        IDataManager dataManager,
        string name,
        ILogger<Nerve<TData, TLink>> logger)
    {
        _logger = logger;
        Name = name;
        _dataAccess = dataManager.Open(name);
        Location = _dataAccess.GetRoot().Wrap(x => x.Dictionary()).GetOrAdd(".nerve");

        NeuronFactory = ActivatorUtilities.CreateInstance<NeuronFactory<TData, TLink>>(serviceProvider, this);
        ConnectionFactory = ActivatorUtilities.CreateInstance<ConnectionFactory<TData, TLink>>(serviceProvider, this);
        var neuron = NeuronFactory.Location
            .Wrap(x => x.Storage())
            .GetOrCreateData(NeuronValue<TData>.Default);
        var connection = ConnectionFactory.Location
            .Wrap(x => x.Storage())
            .GetOrCreateData(new ConnectionValue<TLink> { Next = neuron.Offset });
        Root = ConnectionFactory.GetOrCreate(connection.Offset);
    }

    public void Learn(TLink link, params ReadOnlySpan<TData> data)
    {
        using var enumerator = data.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return;
        }

        var connection = Root.Neuron.GetOrAdd(enumerator.Current, link, Root);
        Interlocked.Increment(ref connection.Neuron.RefValue.Weight);
        Interlocked.Increment(ref connection.RefValue.Weight);
        while (enumerator.MoveNext())
        {
            connection = connection.Neuron.GetOrAdd(enumerator.Current, link, connection);
            Interlocked.Increment(ref connection.Neuron.RefValue.Weight);
            Interlocked.Increment(ref connection.RefValue.Weight);
        }
    }

    public void Sleep()
    {
        throw new NotImplementedException();
    }

    public ThinkResult<TData, TLink> Think(TLink link, params ReadOnlySpan<TData> readOnlySpan)
    {
        var current = Root;
        var result = new NearConnection<TData, TLink>[readOnlySpan.Length];

        for (int index = 0; index < readOnlySpan.Length; index++)
        {
            TData data = readOnlySpan[index];

            var connection = current.Neuron
                .Where(x => x.Previous == current)
                .OrderBy(x => Math.Abs(Comparer<TLink>.Default.Compare(x.RefLink, link)))
                .ThenBy(x => Math.Abs(Comparer<TData>.Default.Compare(x.Neuron.RefData, data)))
                .FirstOrDefault();
            if (connection is null)
                return new(result, null);
            current = connection;
            result[index] = new(data, link, connection);
        }

        var next = current.Neuron
            .Where(x => x.Previous == current)
            .OrderBy(x => Math.Abs(Comparer<TLink>.Default.Compare(x.RefLink, link)))
            .FirstOrDefault();
        return new(result, next is null ? null : new(next.Neuron.RefData, link, next));
    }

    public void Save()
    {
        _dataAccess.Save();
    }

    public async ValueTask SaveAsync(CancellationToken cancellationToken = default)
    {
        await _dataAccess.SaveAsync(cancellationToken);
    }
}
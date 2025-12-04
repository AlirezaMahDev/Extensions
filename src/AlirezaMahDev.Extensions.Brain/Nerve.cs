using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlirezaMahDev.Extensions.Brain;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public partial class
    Nerve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TData> : IDataBlockAccessorSave
    where TData : unmanaged
{
    private readonly ILogger<Nerve<TData>> _logger;
    private readonly Lazy<Neuron<TData>> _rootNeuron;
    private readonly Lazy<RootConnection<TData>> _rootConnection;
    // private readonly ConcurrentDictionary<int, Lazy<RootConnection<TData>>> _rootLevelConnections = [];

    [LoggerMessage(LogLevel.Debug, "{message}")]
    private partial void LogDebug(string message);

    public Nerve(IServiceProvider serviceProvider, IFileService fileService, string name, ILogger<Nerve<TData>> logger)
    {
        _logger = logger;
        Name = name;
        Location = fileService.Access(name).AsData().Root.GetOrAdd(".brain");
        NeuronFactory = ActivatorUtilities.CreateInstance<NeuronFactory<TData>>(serviceProvider, this);
        ConnectionFactory = ActivatorUtilities.CreateInstance<ConnectionFactory<TData>>(serviceProvider, this);
        _rootNeuron = new(() =>
            {
                var rootNeuron = NeuronFactory.GetOrCreate(1);
                LogDebug($"rootNeuron created: {rootNeuron}");
                return rootNeuron;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);
        _rootConnection = new(() =>
        {
            RootConnection<TData> rootConnection = new(this, _rootNeuron.Value);
            LogDebug($"rootConnection created: {rootConnection}");
            return rootConnection;
        }, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public string Name { get; }
    public IDataLocation<> Location { get; }

    public NeuronFactory<TData> NeuronFactory { get; }
    public ConnectionFactory<TData> ConnectionFactory { get; }


    public Neuron<TData> Root => _rootNeuron.Value;
    public RootConnection<TData> RootConnection => _rootConnection.Value;

    // public RootLevelConnection<TData> GetRootConnection(int level) =>
    //     _rootLevelConnections.AddOrUpdate(level, (i, arg) => , (i, lazy, arg3) => , _rootConnection.Value);

    public void Learn(params ReadOnlySpan<TData> data)
    {
        using var enumerator = data.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return;
        }

        var connection = Root.GetOrAdd(enumerator.Current, null);
        Interlocked.Increment(ref connection.Neuron.RefValue.Weight);
        Interlocked.Increment(ref connection.RefValue.Weight);
        while (enumerator.MoveNext())
        {
            connection = connection.Neuron.GetOrAdd(enumerator.Current, connection);
            Interlocked.Increment(ref connection.Neuron.RefValue.Weight);
            Interlocked.Increment(ref connection.RefValue.Weight);
        }
    }

    public void Save()
    {
        NeuronFactory.Save();
        ConnectionFactory.Save();
    }
}
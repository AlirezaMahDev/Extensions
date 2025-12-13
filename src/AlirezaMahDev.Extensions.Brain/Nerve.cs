using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

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
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    private readonly IDataAccess _dataAccess;
    private readonly ILogger<Nerve<TData, TLink>> _logger;

    public string Name { get; }
    public DataLocation<DataPath> Location { get; }

    public NeuronFactory<TData, TLink> NeuronFactory { get; }
    public ConnectionFactory<TData, TLink> ConnectionFactory { get; }

    public INeuron<TData, TLink> RootNeuron { get; }
    public IConnection<TData, TLink> RootConnection { get; }

    public Nerve(IServiceProvider serviceProvider,
        IDataManager dataManager,
        string name,
        ILogger<Nerve<TData, TLink>> logger)
    {
        _logger = logger;
        Name = name;
        _dataAccess = Name.StartsWith("temp:") ? dataManager.OpenTemp() : dataManager.Open(name);
        Location = _dataAccess.GetRoot().Wrap(x => x.Dictionary()).GetOrAdd(".nerve");

        NeuronFactory = ActivatorUtilities.CreateInstance<NeuronFactory<TData, TLink>>(serviceProvider, this);
        ConnectionFactory = ActivatorUtilities.CreateInstance<ConnectionFactory<TData, TLink>>(serviceProvider, this);

        var neuron = NeuronFactory.Location
            .Wrap(x => x.Storage())
            .GetOrCreateData(NeuronValue<TData>.Default);
        var connection = ConnectionFactory.Location
            .Wrap(x => x.Storage())
            .GetOrCreateData(ConnectionValue<TLink>.Default with { Neuron = neuron.Offset });

        RootNeuron = new RootNeuron<TData, TLink>(NeuronFactory.GetOrCreate(neuron.Offset));
        RootConnection = ConnectionFactory.GetOrCreate(connection.Offset);
    }

    public void Learn(TLink link, params ReadOnlySpan<TData> data)
    {
        using var enumerator = data.GetEnumerator();

        var connection = RootConnection;
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

    public Think<TData, TLink>? Think(TLink link, TData[] data)
    {
        var result = new ThinkResult<TData, TLink>();

        ThinkCore(link,
            data.AsMemory(),
            new(default, link, RootConnection, null),
            result
        );

        return result.Think;
    }

    private static void ThinkCore(
        TLink link,
        Memory<TData> input,
        Think<TData, TLink> think,
        ThinkResult<TData, TLink> result)
    {
        if (input.IsEmpty)
        {
            var next = think.Connection
                .Min(Comparer<IConnection<TData, TLink>>.Create((a, b) =>
                    a.CompareTo(link).CompareTo(b.CompareTo(link))));
            if (next is null)
                return;
            result.Add(think.Append(next.Neuron.RefData, next.RefLink, next));
        }

        if (!result.CanAdd(think))
        {
            return;
        }

        TData data = input.Span[0];

        var pain = new DataPairLink<TData, TLink>(data, link);
        var connection = think.Connection;
        var array = connection.ToArray();
        array.Sort((a, b) =>
            a.CompareTo(pain).CompareTo(b.CompareTo(pain)));

        var nextInput = input[1..];

        Parallel.ForEach(array
                .Select(innerConnection => think.Append(data, link, innerConnection))
                .TakeWhile(result.CanAdd),
            innerThink => ThinkCore(link, nextInput, innerThink, result));
    }

    public async ValueTask<Think<TData, TLink>?> ThinkAsync(TLink link,
        CancellationToken cancellationToken = default,
        params TData[] data)
    {
        var result = new ThinkResult<TData, TLink>();

        await ThinkCoreAsync(link,
            data.AsMemory(),
            new(default, link, RootConnection, null),
            result,
            cancellationToken);

        return result.Think;
    }

    private static async Task ThinkCoreAsync(
        TLink link,
        Memory<TData> input,
        Think<TData, TLink> think,
        ThinkResult<TData, TLink> result,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (input.IsEmpty)
        {
            var next = think.Connection
                .Min(Comparer<IConnection<TData, TLink>>.Create((a, b) =>
                    a.CompareTo(link).CompareTo(b.CompareTo(link))));
            if (next is null)
                return;
            result.Add(think.Append(next.Neuron.RefData, next.RefLink, next));
        }

        if (!result.CanAdd(think))
        {
            return;
        }

        await Task.Yield();

        TData data = input.Span[0];

        var pain = new DataPairLink<TData, TLink>(data, link);
        var connection = think.Connection;
        var span = connection.ToArray().AsSpan();
        span.Sort((a, b) =>
            a.CompareTo(pain).CompareTo(b.CompareTo(pain)));

        using var tasks = MemoryPool<Task>.Shared.Rent(span.Length);
        var taskCount = 0;
        var nextInput = input[1..];

        for (int i = 0; i < span.Length; i++)
        {
            var innerConnection = span[i];
            var innerThink = think.Append(data, link, innerConnection);
            if (result.CanAdd(innerThink))
            {
                tasks.Memory.Span[i] = ThinkCoreAsync(link, nextInput, innerThink, result, cancellationToken);
                taskCount++;
            }
            else
            {
                break;
            }
        }

        await Task.WhenAll(tasks.Memory.Span[..taskCount]);
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
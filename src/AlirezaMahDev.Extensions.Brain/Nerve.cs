using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

using Microsoft.Extensions.DependencyInjection;

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

    public NerveCache Cache { get; }

    public string Name { get; }
    public DataLocation<DataPath> Location { get; }

    public NeuronFactory<TData, TLink> NeuronFactory { get; }
    public ConnectionFactory<TData, TLink> ConnectionFactory { get; }

    public IRootNeuron<TData, TLink> RootNeuron { get; }
    public IConnection<TData, TLink> RootConnection { get; }

    public Nerve(IServiceProvider serviceProvider,
        IDataManager dataManager,
        string name)
    {
        Cache = new();

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
        RootConnection = new RootConnection<TData, TLink>(ConnectionFactory.GetOrCreate(connection.Offset));
    }

    public void Learn(ReadOnlyMemoryValue<TLink> link, ReadOnlyMemory<TData> data)
    {
        var connection = RootConnection;
        foreach (var current in data.Span)
        {
            var neuron = connection.GetNeuron();
            connection = neuron.FindOrAdd(current, link, connection);
            neuron = connection.GetNeuron();

            Interlocked.Increment(ref neuron.Location.RefValue.Weight);
            Interlocked.Increment(ref connection.Location.RefValue.Weight);
        }
    }

    public async ValueTask LearnAsync(ReadOnlyMemoryValue<TLink> link,
        ReadOnlyMemory<TData> data,
        CancellationToken cancellationToken = default)
    {
        var connection = RootConnection;
        for (var i = 0; i < data.Length; i++)
        {
            var current = data.Span[i];
            var neuron = await connection.GetNeuronAsync(cancellationToken);
            connection = await neuron.FindOrAddAsync(current, link, connection, cancellationToken);
            neuron = await connection.GetNeuronAsync(cancellationToken);
            Interlocked.Increment(ref neuron.Location.RefValue.Weight);
            Interlocked.Increment(ref connection.Location.RefValue.Weight);
        }
    }

    public void Sleep()
    {
        throw new NotImplementedException();
    }

    public ValueTask SleepAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Think<TData, TLink>? Think(ReadOnlyMemoryValue<TLink> link, ReadOnlyMemory<TData> data)
    {
        var result = new ThinkResult<TData, TLink>();

        ThinkCore(link,
            data,
            new(default, link.Value, RootConnection, null),
            result
        );

        return result.Think;
    }

    private static void ThinkCore(
        ReadOnlyMemoryValue<TLink> link,
        ReadOnlyMemory<TData> input,
        Think<TData, TLink> think,
        ThinkResult<TData, TLink> result)
    {
        if (input.IsEmpty)
        {
            var next = think.Connection
                .Min(Comparer<IConnection<TData, TLink>>.Create((a, b) =>
                    a.CompareTo(link.Value).CompareTo(b.CompareTo(link.Value))));
            if (next is null)
                return;
            result.Add(think.Append(next.GetNeuron().RefData, next.RefLink, next));
        }

        if (!result.CanAdd(think))
        {
            return;
        }

        var data = input.Span[0];

        var pain = new DataPairLink<TData, TLink>(data, link.Value);
        var connection = think.Connection;
        var array = connection.ToArray();
        array.Sort((a, b) =>
            a.CompareTo(pain).CompareTo(b.CompareTo(pain)));

        var nextInput = input[1..];

        Parallel.ForEach(array
                .Select(innerConnection => think.Append(data, link.Value, innerConnection))
                .TakeWhile(result.CanAdd),
            innerThink => ThinkCore(link, nextInput, innerThink, result));
    }

    public async ValueTask<Think<TData, TLink>?> ThinkAsync(ReadOnlyMemoryValue<TLink> link,
        ReadOnlyMemory<TData> data,
        CancellationToken cancellationToken = default)
    {
        var result = new ThinkResult<TData, TLink>();

        await ThinkCoreAsync(link,
            data,
            new(default, link.Value, RootConnection, null),
            result,
            cancellationToken);

        return result.Think;
    }

    private static async Task ThinkCoreAsync(
        ReadOnlyMemoryValue<TLink> link,
        ReadOnlyMemory<TData> input,
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
                    a.CompareTo(link.Value).CompareTo(b.CompareTo(link.Value))));
            if (next is null)
                return;
            result.Add(await think.AppendAsync((await next.GetNeuronAsync(cancellationToken)).RefData,
                next.RefLink,
                next,
                cancellationToken));
        }

        if (!result.CanAdd(think))
        {
            return;
        }

        await Task.Yield();

        var data = input.Span[0];

        var pain = new DataPairLink<TData, TLink>(data, link.Value);
        var connection = think.Connection;
        var memory = connection.ToArray().AsMemory();
        memory.Span.Sort((a, b) =>
            a.CompareTo(pain).CompareTo(b.CompareTo(pain)));

        using var tasks = MemoryPool<Task>.Shared.Rent(memory.Length);
        var taskCount = 0;
        var nextInput = input[1..];

        for (var i = 0; i < memory.Length; i++)
        {
            var innerConnection = memory.Span[i];
            var innerThink = await think.AppendAsync(data, link.Value, innerConnection, cancellationToken);
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

    public void Flush()
    {
        _dataAccess.Flush();
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        await _dataAccess.FlushAsync(cancellationToken);
    }
}
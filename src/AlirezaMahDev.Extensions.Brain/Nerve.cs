using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

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
    public IDataAccess Access { get; }
    public INerveCache<TData, TLink> Cache { get; }

    public string Name { get; }
    public DataLocation<DataPath> Location { get; }
    public DataLocation<DataPath> ConnectionLocation { get; }
    public DataLocation<DataPath> NeuronLocation { get; }
    public Neuron<TData, TLink> Neuron { get; }
    public NeuronWrap<TData, TLink> NeuronWrap { get; }
    public Connection<TData, TLink> Connection { get; }
    public ConnectionWrap<TData, TLink> ConnectionWrap { get; }

    public Nerve(IDataManager dataManager, string name)
    {
        Cache = new NerveCache<TData, TLink>();

        Name = name;
        Access = Name.StartsWith("temp:") ? dataManager.OpenTemp() : dataManager.Open(name);
        Location = Access.Root.Wrap(Access, x => x.Dictionary()).GetOrAdd(".nerve");
        ConnectionLocation = Location.Wrap(Access, x => x.Dictionary()).GetOrAdd(".connection");
        NeuronLocation = Location.Wrap(Access, x => x.Dictionary()).GetOrAdd(".neuron");

        var neuron = NeuronLocation
            .Wrap(Access, x => x.Storage())
            .GetOrCreateData(NeuronValue<TData>.Default);
        var connection = ConnectionLocation
            .Wrap(Access, x => x.Storage())
            .GetOrCreateData(ConnectionValue<TLink>.Default with { Neuron = neuron.Offset });

        Neuron = new(neuron.Offset);
        NeuronWrap = Neuron.Wrap(this);
        Connection = new(connection.Offset);
        ConnectionWrap = Connection.Wrap(this);
    }

    public void Learn(ReadOnlyMemoryValue<TLink> link, ReadOnlyMemory<TData> data)
    {
        var connectionWrap = ConnectionWrap;
        for (var i = 0; i < data.Length; i++)
        {
            var neuron = this.FindOrAddNeuron(data.Span[i]);
            var connection = connectionWrap.FindOrAdd(neuron, link.Value);
            connectionWrap = connection.Wrap(this);
            Interlocked.Increment(ref connectionWrap.Location.RefValue.Weight);
            Interlocked.Increment(ref connectionWrap.NeuronWrap.Location.RefValue.Weight);
        }
    }

    public async ValueTask LearnAsync(ReadOnlyMemoryValue<TLink> link,
        ReadOnlyMemory<TData> data,
        CancellationToken cancellationToken = default)
    {
        var connectionWrap = ConnectionWrap;
        for (var i = 0; i < data.Length; i++)
        {
            var neuron = await this.FindOrAddNeuronAsync(data.Span[i], cancellationToken);
            var connection = await connectionWrap.FindOrAddAsync(neuron, link, cancellationToken);
            connectionWrap = connection.Wrap(this);
            Interlocked.Increment(ref connectionWrap.Location.RefValue.Weight);
            Interlocked.Increment(ref connectionWrap.NeuronWrap.Location.RefValue.Weight);
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
            new(default, link.Value, Connection.Wrap(this), null),
            result
        );

        return result.Think;
    }

    private void ThinkCore(
        ReadOnlyMemoryValue<TLink> link,
        ReadOnlyMemory<TData> input,
        Think<TData, TLink> think,
        ThinkResult<TData, TLink> result)
    {
        if (input.IsEmpty)
        {
            var next = think.Connection.GetSubConnections()
                .Min(Comparer<Connection<TData, TLink>>.Create((a, b) =>
                    a.Wrap(this).CompareTo(link.Value).CompareTo(b.Wrap(this).CompareTo(link.Value))))
                .NullWhenDefault();
            if (!next.HasValue)
                return;
            var connectionWrap = next.Value.Wrap(this);
            result.Add(think.Append(connectionWrap.Neuron.Wrap(this).RefData,
                connectionWrap.RefLink,
                connectionWrap));
        }

        if (!result.CanAdd(think))
        {
            return;
        }

        var data = input.Span[0];

        var pain = new DataPairLink<TData, TLink>(data, link.Value);
        var connection = think.Connection;
        var array = connection.GetSubConnectionsWrap().ToArray();
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
            new(default, link.Value, Connection.Wrap(this), null),
            result,
            cancellationToken);

        return result.Think;
    }

    private async Task ThinkCoreAsync(
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
            var next = think.Connection.GetSubConnections()
                .Min(Comparer<Connection<TData, TLink>>.Create((a, b) =>
                    a.Wrap(this).CompareTo(link.Value).CompareTo(b.Wrap(this).CompareTo(link.Value))))
                .NullWhenDefault();
            if (!next.HasValue)
                return;
            var nextWrap = next.Value.Wrap(this);
            result.Add(await think.AppendAsync(nextWrap.Neuron.Wrap(this).RefData,
                nextWrap.RefLink,
                nextWrap,
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
        var memory = connection.GetSubConnections().ToArray().AsMemory();
        memory.Span.Sort((a, b) =>
            a.Wrap(this).CompareTo(pain).CompareTo(b.Wrap(this).CompareTo(pain)));

        using var tasks = MemoryPool<Task>.Shared.Rent(memory.Length);
        var taskCount = 0;
        var nextInput = input[1..];

        for (var i = 0; i < memory.Length; i++)
        {
            var innerConnection = memory.Span[i];
            var innerThink = await think.AppendAsync(data, link.Value, innerConnection.Wrap(this), cancellationToken);
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
        Access.Flush();
    }
}
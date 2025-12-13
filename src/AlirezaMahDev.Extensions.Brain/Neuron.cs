using System.Collections.Concurrent;
using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class Neuron<TData, TLink>(NerveArgs<TData, TLink> args) : INeuron<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    private readonly DataLocation<NeuronValue<TData>> _location =
        args.Nerve.Location.Access.Read<NeuronValue<TData>>(args.Offset);

    protected readonly ConcurrentDictionary<NeuronCacheKey<TData, TLink>, Lazy<NerveArgs<TData, TLink>>> _cache = [];

    protected readonly Lock _lock = new();
    protected internal readonly Nerve<TData, TLink> _nerve = args.Nerve;

    public long Offset => _location.Offset;

    public ref NeuronValue<TData> RefValue => ref _location.RefValue;
    public ref TData RefData => ref RefValue.Data;

    public IConnection<TData, TLink>? Connection =>
        RefValue.Connection != -1
            ? _nerve.ConnectionFactory.GetOrCreate(new NerveArgs<TData, TLink>(_nerve, RefValue.Connection))
            : null;

    public virtual IConnection<TData, TLink> GetOrAdd(TData data, TLink link, IConnection<TData, TLink>? connection) =>
        _nerve.ConnectionFactory.GetOrCreate(
            _cache.GetOrAdd(
                    new(data, link, connection?.Offset ?? -1),
                    static (key, args) =>
                        new(() => new(
                                args.neuron._nerve,
                                args.neuron.Add(key.Data, key.Link, args.connection).Offset
                            ),
                            LazyThreadSafetyMode.ExecutionAndPublication),
                    (neuron: this, connection))
                .Value
        );

    protected virtual IConnection<TData, TLink> Add(TData data, TLink link, IConnection<TData, TLink>? previous)
    {
        using var scope = _lock.EnterScope();

        if (_cache.TryGetValue(new(data, link, previous?.Offset ?? -1), out var item) && item.IsValueCreated)
            return _nerve.ConnectionFactory.GetOrCreate(item.Value);

        if (Connection?.FirstOrDefault(x =>
                x.Neuron.RefData.Equals(data) && x.RefLink.Equals(link) && x.Previous?.Offset == previous?.Offset) is
            { } connection)
            return connection;

        connection = _nerve.RootNeuron.GetOrAdd(data, link, previous);

        var next = RefValue.Connection;
        var connectionItem = _nerve.Location.Access
            .Create(ConnectionValue<TLink>.Default with
            {
                Neuron = connection.Neuron.Offset,
                Connection = next,
                Previous = previous?.Offset ?? -1,
                Link = link
            });
        RefValue.Connection = connectionItem.Offset;

        return _nerve.ConnectionFactory.GetOrCreate(connectionItem.Offset);
    }

    public IEnumerable<IConnection<TData, TLink>> GetConnections()
    {
        var current = Connection;
        while (current is not null)
        {
            _ = _cache.GetOrAdd(
                new(current.Neuron.RefData, current.RefLink, current.Previous?.Offset ?? -1),
                static (_, arg) => new(arg),
                new NerveArgs<TData, TLink>(_nerve, current.Offset));
            yield return current;
            current = current.Next;
        }
    }
}
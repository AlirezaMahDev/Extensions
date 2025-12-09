using System.Collections;
using System.Collections.Concurrent;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class Neuron<TData, TLink> : INeuron<TData, TLink>
    where TData : unmanaged
    where TLink : unmanaged
{
    private readonly Lock _lock = new();
    private readonly NerveArgs<TData, TLink> _args;
    private readonly DataLocation<NeuronValue<TData>> _location;

    private readonly Lazy<ConcurrentDictionary<NeuronCacheKey<TData, TLink>, Lazy<NerveArgs<TData, TLink>>>> _cache;

    public Neuron(NerveArgs<TData, TLink> args)
    {
        _args = args;
        _location = args.Nerve.Location.Access.Read<NeuronValue<TData>>(args.Offset);
        _cache = new(() =>
                new(this
                    .Select(x =>
                        new KeyValuePair<NeuronCacheKey<TData, TLink>, Lazy<NerveArgs<TData, TLink>>>(
                            new(x.Neuron.RefData, x.RefLink, x.Previous),
                            new(new NerveArgs<TData, TLink>(args.Nerve, x.Offset))
                        )
                    )
                ),
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }

    public long Offset => _args.Offset;

    public ref NeuronValue<TData> RefValue => ref _location.RefValue;
    public ref TData RefData => ref RefValue.Data;

    public IConnection<TData, TLink>? Connection =>
        RefValue.Connection != -1
            ? _args.Nerve.ConnectionFactory.GetOrCreate(_args with { Offset = RefValue.Connection })
            : null;

    public IConnection<TData, TLink> GetOrAdd(TData data, TLink link, IConnection<TData, TLink>? connection) =>
        _args.Nerve.ConnectionFactory.GetOrCreate(
            _cache.Value.GetOrAdd(
                    new(data, link, connection),
                    static (key, neuron) =>
                        new(() => new(
                                neuron._args.Nerve,
                                neuron.Add(key.Data, key.Link, key.Connection).Offset
                            ),
                            LazyThreadSafetyMode.ExecutionAndPublication),
                    this)
                .Value
        );

    private Connection<TData, TLink> Add(TData data, TLink link, IConnection<TData, TLink>? previous)
    {
        using var scope = _lock.EnterScope();
        var to = _args.Nerve.NeuronFactory.GetOrCreate(data);
        var previousId = previous?.Offset ?? 0;

        var next = RefValue.Connection;
        var connectionItem = _args.Nerve.Location.Access.Create<ConnectionValue<TLink>>();
        connectionItem.RefValue = new()
        {
            Neuron = to.Offset,
            Previous = previousId,
            Next = next,
            Weight = 1,
            Score = 1,
            Link = link
        };
        RefValue.Connection = connectionItem.Offset;

        return _args.Nerve.ConnectionFactory.GetOrCreate(connectionItem.Offset);
    }

    public IEnumerator<IConnection<TData, TLink>> GetEnumerator()
    {
        var current = Connection;
        while (current is not null)
        {
            yield return current;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
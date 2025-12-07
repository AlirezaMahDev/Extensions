using System.Collections;
using System.Collections.Concurrent;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class Neuron<TData> : INeuron<TData>
    where TData : unmanaged
{
    private readonly Lock _lock = new();
    private readonly NerveArgs<TData> _args;
    private readonly DataLocation<NeuronValue<TData>> _location;

    private readonly Lazy<ConcurrentDictionary<NeuronCacheKey<TData>, Lazy<NerveArgs<TData>>>> _cache;

    public Neuron(NerveArgs<TData> args)
    {
        _args = args;
        _location = args.Nerve.Location.Access.Read<NeuronValue<TData>>(args.Offset);
        _cache = new(() =>
                new(this
                    .Select(x =>
                        new KeyValuePair<NeuronCacheKey<TData>, Lazy<NerveArgs<TData>>>(
                            new(x.Neuron.RefData, x.Previous),
                            new(new NerveArgs<TData>(args.Nerve, x.Offset))
                        )
                    )
                ),
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }

    public long Offset => _args.Offset;

    public ref NeuronValue<TData> RefValue => ref _location.RefValue;
    public ref TData RefData => ref RefValue.Data;

    public IConnection<TData>? Connection =>
        RefValue.Connection != -1
            ? _args.Nerve.ConnectionFactory.GetOrCreate(_args with { Offset = RefValue.Connection })
            : null;

    public IConnection<TData> GetOrAdd(TData data, IConnection<TData>? connection) =>
        _args.Nerve.ConnectionFactory.GetOrCreate(
            _cache.Value.GetOrAdd(
                    new(data, connection),
                    static (key, neuron) =>
                        new(() => new(
                                neuron._args.Nerve,
                                neuron.Add(key.Data, key.Connection).Offset
                            ),
                            LazyThreadSafetyMode.ExecutionAndPublication),
                    this)
                .Value
        );

    private Connection<TData> Add(TData data, IConnection<TData>? previous)
    {
        using var scope = _lock.EnterScope();
        var to = _args.Nerve.NeuronFactory.GetOrCreate(data);
        var previousId = previous?.Offset ?? 0;

        var next = RefValue.Connection;
        var connectionItem = _args.Nerve.Location.Access.Create<ConnectionValue>();
        connectionItem.RefValue = new()
        {
            Neuron = to.Offset,
            Previous = previousId,
            Next = next,
            Weight = 1,
            Score = 1
        };
        RefValue.Connection = connectionItem.Offset;

        return _args.Nerve.ConnectionFactory.GetOrCreate(connectionItem.Offset);
    }

    public IEnumerator<IConnection<TData>> GetEnumerator()
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
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
public class Neuron<TData> : INeuron<TData>
    where TData : unmanaged
{
    private readonly Lock _lock = new();
    private readonly DataLocation<NeuronValue<TData>> _neuronItem;
    private readonly NerveArgs<TData> _args;

    private readonly Lazy<ConcurrentDictionary<NeuronCacheKey<TData>, Lazy<NerveArgs<TData>>>>
        _cache;

    public Neuron(NerveArgs<TData> args)
    {
        _args = args;
        _neuronItem = args.Nerve.NeuronFactory.NeuronStack.Items.GetOrAdd(args.Id);
        _cache = new(() =>
                new(this
                    .Select(x =>
                        new KeyValuePair<NeuronCacheKey<TData>, Lazy<NerveArgs<TData>>>(
                            new(x.Neuron.RefData, x.Previous),
                            new(new NerveArgs<TData>(args.Nerve, x.Id))
                        )
                    )
                ),
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }

    public int Id => _args.Id;

    public ref NeuronValue<TData> RefValue => ref _neuronItem.RefValue;
    public ref TData RefData => ref RefValue.Data;

    public Connection<TData>? Connection =>
        RefValue.Connection.IsNotDefault
            ? _args.Nerve.ConnectionFactory.GetOrCreate(_args with { Id = RefValue.Connection })
            : null;

    public Connection<TData> GetOrAdd(TData data, Connection<TData>? connection)
    {
        return _args.Nerve.ConnectionFactory.GetOrCreate(
            _cache.Value.GetOrAdd(
                    new(data, connection),
                    static (key, neuron) =>
                        new(() => new(
                                neuron._args.Nerve,
                                neuron.Add(key.Data, key.Connection).Id
                            ),
                            LazyThreadSafetyMode.ExecutionAndPublication),
                    this)
                .Value
        );
    }

    private Connection<TData> Add(TData data, Connection<TData>? previous)
    {
        using var scope = _lock.EnterScope();
        var to = _args.Nerve.NeuronFactory.GetOrCreate(data);
        var previousId = previous?.Id ?? 0;

        var next = RefValue.Connection;
        var connectionItem = _args.Nerve.ConnectionFactory.ConnectionStack.Items.Add();
        connectionItem.RefValue = new()
        {
            Neuron = to.Id,
            Previous = previousId,
            Next = next,
            Weight = 1,
            Score = 1
        };
        RefValue.Connection = connectionItem.Item.Index;

        return _args.Nerve.ConnectionFactory.GetOrCreate(connectionItem.Item.Index);
    }

    public IEnumerator<Connection<TData>> GetEnumerator()
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
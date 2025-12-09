using System.Collections.Concurrent;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.Brain;

class NeuronFactory<TData, TLink> : ParameterInstanceFactory<Neuron<TData, TLink>, NerveArgs<TData, TLink>>
    where TData : unmanaged
    where TLink : unmanaged
{
    private readonly Nerve<TData, TLink> _nerve;
    private readonly Lazy<ConcurrentDictionary<TData, Lazy<NerveArgs<TData, TLink>>>> _cache;

    public DataLocation<DataPath> Location { get; }

    public NeuronFactory(IServiceProvider provider,
        Nerve<TData, TLink> nerve) : base(provider)
    {
        _nerve = nerve;
        Location = nerve.Location.Wrap(x => x.Dictionary()).GetOrAdd(".neuron");

        _cache = new(() =>
                new(nerve.Root.Neuron
                    .Select(x =>
                        new KeyValuePair<TData, Lazy<NerveArgs<TData, TLink>>>(
                            x.Neuron.RefData,
                            new(() => new(nerve, x.Offset),
                                LazyThreadSafetyMode.ExecutionAndPublication)
                        )
                    )
                ),
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }

    public Neuron<TData, TLink> GetOrCreate(TData data)
    {
        return GetOrCreate(
            _cache.Value.GetOrAdd(data,
                    static (data, factory) =>
                        new(() => new(
                                factory._nerve,
                                factory.Location.Access.Create(new NeuronValue<TData>
                                    {
                                        Data = data,
                                        Connection = 0,
                                        Score = 1,
                                        Weight = 1
                                    })
                                    .Offset),
                            LazyThreadSafetyMode.ExecutionAndPublication),
                    this)
                .Value);
    }

    public Neuron<TData, TLink> GetOrCreate(int id)
    {
        return GetOrCreate(new NerveArgs<TData, TLink>(_nerve, id));
    }

    public override Neuron<TData, TLink> GetOrCreate(NerveArgs<TData, TLink> parameter)
    {
        var result = base.GetOrCreate(parameter);
        _cache.Value.GetOrAdd(result.RefData, _ => new(() => parameter, LazyThreadSafetyMode.ExecutionAndPublication));
        return result;
    }
}
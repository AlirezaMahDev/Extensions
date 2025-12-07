using System.Collections.Concurrent;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.Brain;

class NeuronFactory<TData> : ParameterInstanceFactory<Neuron<TData>, NerveArgs<TData>>
    where TData : unmanaged
{
    private readonly Nerve<TData> _nerve;
    private readonly Lazy<ConcurrentDictionary<TData, Lazy<NerveArgs<TData>>>> _cache;

    public DataLocation<DataPath> Location { get; }

    public NeuronFactory(IServiceProvider provider,
        Nerve<TData> nerve) : base(provider)
    {
        _nerve = nerve;
        Location = nerve.Location.Wrap(x => x.Dictionary()).GetOrAdd(".neuron");

        _cache = new(() =>
                new(nerve.Root.Neuron
                    .Select(x =>
                        new KeyValuePair<TData, Lazy<NerveArgs<TData>>>(
                            x.Neuron.RefData,
                            new(() => new(nerve, x.Offset),
                                LazyThreadSafetyMode.ExecutionAndPublication)
                        )
                    )
                ),
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }

    public Neuron<TData> GetOrCreate(TData data)
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

    public Neuron<TData> GetOrCreate(int id)
    {
        return GetOrCreate(new NerveArgs<TData>(_nerve, id));
    }

    public override Neuron<TData> GetOrCreate(NerveArgs<TData> parameter)
    {
        var result = base.GetOrCreate(parameter);
        _cache.Value.GetOrAdd(result.RefData, _ => new(() => parameter, LazyThreadSafetyMode.ExecutionAndPublication));
        return result;
    }
}
using System.Collections.Concurrent;

using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.Brain;

public class NeuronFactory<TData> : ParameterInstanceFactory<Neuron<TData>, NerveArgs<TData>>
    where TData : unmanaged
{
    private readonly Nerve<TData> _nerve;
    private readonly Lazy<ConcurrentDictionary<TData, Lazy<NerveArgs<TData>>>> _cache;

    public NeuronFactory(IServiceProvider provider,
        Nerve<TData> nerve) : base(provider)
    {
        _nerve = nerve;
        NeuronStack = nerve.Location.GetOrAdd(".neurons").AsStack().As<NeuronValue<TData>>();

        _cache = new(() =>
                new(NeuronStack.Items
                    .Select(x =>
                        new KeyValuePair<TData, Lazy<NerveArgs<TData>>>(
                            x.RefValue.Data,
                            new(new NerveArgs<TData>(nerve, x.Item.Index))
                        )
                    )
                ),
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }

    public StackAccess<NeuronValue<TData>> NeuronStack { get; }

    public Neuron<TData> GetOrCreate(TData data)
    {
        return GetOrCreate(
            _cache.Value.GetOrAdd(data,
                    static (data, factory) =>
                        new(() => new(
                                factory._nerve,
                                factory.NeuronStack.Items.Add(x => x.RefValue =
                                        new()
                                        {
                                            Data = data,
                                            Connection = 0,
                                            Score = 1,
                                            Weight = 1
                                        })
                                    .Item
                                    .Index),
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
        _cache.Value.GetOrAdd(result.RefData, static (_, parameter) => new(parameter), parameter);
        return result;
    }

    public void Save()
    {
        NeuronStack.Stack.Save();
    }
}
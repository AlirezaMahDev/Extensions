using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public Neuron? FindNeuron(in TData data)
        {
            var cacheKey = nerve.CreateNeuronCacheKey(in data);
            return nerve.FindNeuronCore(in cacheKey, in data);
        }

        public Neuron? FindNeuronCore(in NerveCacheKey cacheKey, in TData data)
        {
            if (nerve.TryGetNeuronCacheCore(in cacheKey, out var offset))
                return new(offset.Value);

            var localData = data;
            var connection = nerve.NeuronWrap
                .GetUnloadedConnectionsWrap()
                .FirstOrDefault(x => x.Neuron.Wrap(nerve).RefData.Equals(localData))
                .NullWhenDefault();
            if (!connection.HasValue)
                return null;

            offset = connection.Value.RefValue.Neuron;
            nerve.TrySetNeuronCacheCore(in cacheKey, offset.Value);
            return new(offset.Value);
        }

        public Neuron FindOrAddNeuron(ReadOnlyMemoryValue<TData> data)
        {
            var cacheKey = nerve.CreateNeuronCacheKey(in data.Value);
            return nerve.FindNeuronCore(in cacheKey, in data.Value) ?? nerve.AddNeuronCore(cacheKey, data);
        }

        private Neuron AddNeuronCore(NerveCacheKey cacheKey, ReadOnlyMemoryValue<TData> data)
        {
            return nerve.Neuron.Wrap(nerve)
                .Lock(neuronDataLocation =>
                {
                    if (nerve.FindNeuronCore(in cacheKey, in data.Value) is { } neuron)
                        return neuron;

                    var neuronValue = nerve.Access
                        .Create(NeuronValue<TData>.Default with { Data = data.Value });

                    var neuronDataLocationWrap = neuronDataLocation.Wrap(nerve.Access);
                    var connectionValue = nerve.Access.Create(ConnectionValue<TLink>.Default with
                    {
                        Neuron = neuronValue.Offset,
                        Next = neuronDataLocationWrap.RefValue.Connection
                    });

                    neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;

                    nerve.TrySetNeuronCacheCore(in cacheKey, neuronValue.Offset);
                    return new(neuronValue.Offset);
                });
        }

        public async ValueTask<Neuron> FindOrAddNeuronAsync(ReadOnlyMemoryValue<TData> data,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = nerve.CreateNeuronCacheKey(in data.Value);
            return nerve.FindNeuronCore(in cacheKey, in data.Value) ??
                   await nerve.AddNeuronAsyncCore(cacheKey, data, cancellationToken);
        }

        private async ValueTask<Neuron> AddNeuronAsyncCore(NerveCacheKey cacheKey,
            ReadOnlyMemoryValue<TData> data,
            CancellationToken cancellationToken)
        {
            return await nerve.Neuron.Wrap(nerve)
                .LockAsync(neuronDataLocation =>
                    {
                        if (nerve.FindNeuronCore(in cacheKey, in data.Value) is { } neuron)
                            return neuron;

                        var neuronValue =
                            nerve.Access.Create(NeuronValue<TData>.Default with { Data = data.Value });

                        var neuronDataLocationWrap = neuronDataLocation.Wrap(nerve.Access);
                        var connectionValue = nerve.Access
                            .Create(ConnectionValue<TLink>.Default with
                            {
                                Neuron = neuronValue.Offset,
                                Next = neuronDataLocationWrap.RefValue.Connection
                            });

                        neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;

                        nerve.TrySetNeuronCacheCore(in cacheKey, neuronValue.Offset);
                        return new(neuronValue.Offset);
                    },
                    cancellationToken);
        }
    }
}
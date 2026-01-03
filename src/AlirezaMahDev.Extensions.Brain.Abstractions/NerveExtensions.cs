using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public Neuron<TData, TLink>? FindNeuron(in TData data)
        {
            var cacheKey = nerve.Cache.CreateNeuronCacheKey(in data);
            return nerve.FindNeuronCore(in cacheKey, in data);
        }

        public Neuron<TData, TLink>? FindNeuronCore(in NerveCacheKey cacheKey, in TData data)
        {
            if (nerve.Cache.TryGetNeuronCacheCore(in cacheKey, out var offset))
                return new(offset.Value);

            var localData = data;
            var connection = nerve.NeuronWrap
                .GetUnloadedConnections()
                .FirstOrDefault(x => x.Wrap(nerve).Neuron.Wrap(nerve).RefData.Equals(localData))
                .NullWhenDefault();
            if (!connection.HasValue)
                return null;

            offset = connection.Value.Wrap(nerve).RefValue.Neuron;
            nerve.Cache.TrySetNeuronCacheCore(in cacheKey, offset.Value);
            return new(offset.Value);
        }

        public Neuron<TData, TLink> FindOrAddNeuron(ReadOnlyMemoryValue<TData> data)
        {
            var cacheKey = nerve.Cache.CreateNeuronCacheKey(in data.Value);
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

                    nerve.Cache.TrySetNeuronCacheCore(in cacheKey, neuronValue.Offset);
                    return new(neuronValue.Offset);
                });
        }

        public async ValueTask<Neuron<TData, TLink>> FindOrAddNeuronAsync(ReadOnlyMemoryValue<TData> data,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = nerve.Cache.CreateNeuronCacheKey(in data.Value);
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

                        nerve.Cache.TrySetNeuronCacheCore(in cacheKey, neuronValue.Offset);
                        return new(neuronValue.Offset);
                    },
                    cancellationToken);
        }
    }
}
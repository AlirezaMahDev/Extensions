using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public Neuron? FindNeuronCore(in NerveCacheKey cacheKey, in TData data)
        {
            if (nerve.TryGetNeuronCacheCore(in cacheKey, out var offset))
                return new(offset.Value);

            var localData = data;
            var cellMemory = nerve.NeuronWrap
                .GetUnloadedConnectionsWrap();
            var connection = cellMemory
                .FirstOrDefault(x => x.Neuron.Wrap(nerve).RefData.Equals(localData))
                .NullWhenDefault();
            if (!connection.HasValue)
                return null;

            offset = connection.Value.RefValue.Neuron;
            nerve.TrySetNeuronCacheCore(in cacheKey, offset.Value);
            return new(offset.Value);
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
            return await nerve.NeuronWrap
                .LockAsync(valueWrap =>
                    {
                        if (nerve.FindNeuronCore(in cacheKey, in data.Value) is { } neuron)
                            return neuron;

                        var neuronValue =
                            nerve.Access.Create(NeuronValue<TData>.Default with { Data = data.Value });
                        Interlocked.Increment(ref nerve.Counter.RefValue.NeuronCount);

                        var connectionValue = nerve.Access
                            .Create(ConnectionValue<TLink>.Default with
                            {
                                Neuron = neuronValue.Offset,
                                Next = valueWrap.RefValue.Connection,
                                NextCount = nerve.NeuronWrap.ConnectionWrap?.RefValue.NextCount + 1 ?? 0
                            });
                        Interlocked.Increment(ref nerve.Counter.RefValue.ConnectionCount);

                        valueWrap.RefValue.Connection = connectionValue.Offset;

                        nerve.TrySetNeuronCacheCore(in cacheKey, neuronValue.Offset);
                        return new(neuronValue.Offset);
                    },
                    cancellationToken);
        }
    }
}
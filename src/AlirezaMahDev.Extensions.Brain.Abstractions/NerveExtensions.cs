namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public Optional<Neuron> FindNeuronCore(in NerveCacheKey cacheKey, in TData data)
        {
            if (nerve.TryGetNeuronCacheCore(in cacheKey, out var offset))
            {
                return Optional<Neuron>.From(new(offset.Value));
            }

            var localData = data;
            var cellMemory = nerve.RootNeuronWrap
                .GetUnloadedConnectionsWrap();
            var connection = cellMemory
                .FirstOrDefault(x => x.Neuron.Wrap(nerve).RefData.Equals(localData))
                .NullWhenDefault();
            if (!connection.HasValue)
            {
                return Optional<Neuron>.Null;
            }

            offset = connection.Value.RefValue.Neuron;
            nerve.SetNeuronCacheCore(in cacheKey, offset.Value);
            return Optional<Neuron>.From(new(offset.Value));
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public Neuron FindOrAddNeuron(in TData data)
        {
            var cacheKey = nerve.CreateNeuronCacheKey(in data);
            return nerve.FindNeuronCore(in cacheKey, in data) is { HasValue: true } optional
                ? optional.Value
                : nerve.AddNeuronCore(in cacheKey, in data);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private Neuron AddNeuronCore(
            in NerveCacheKey cacheKey,
            in TData data)
        {
            using var @lock = nerve.RootNeuronWrap.Lock();
            if (nerve.FindNeuronCore(in cacheKey, in data) is { HasValue: true } neuron)
            {
                return neuron.Value;
            }

            nerve.Access.Create(NeuronValue<TData>.Default with { Data = data }, out var neuronValue);
            Interlocked.Increment(ref nerve.Counter.RefValue.NeuronCount);

            var location = nerve.RootNeuronWrap.Location;
            nerve.Access
                .Create(ConnectionValue<TLink>.Default with
                {
                    Neuron = neuronValue.Offset,
                    Next = location.RefValue.Connection,
                    NextCount = nerve.RootNeuronWrap.ConnectionWrap is { HasValue: true } connectionWrap
                            ? connectionWrap.Value.RefValue.NextCount + 1
                            : 0
                },
                    out var connectionValue);
            Interlocked.Increment(ref nerve.Counter.RefValue.ConnectionCount);

            location.RefValue.Connection = connectionValue.Offset;

            nerve.SetNeuronCacheCore(in cacheKey, in neuronValue.Offset);
            return new(neuronValue.Offset);
        }
    }
}
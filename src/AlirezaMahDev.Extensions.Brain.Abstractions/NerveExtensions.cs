namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public Optional<Neuron> FindNeuronCore(ref readonly NerveCacheKey cacheKey, ref readonly TData data)
        {
            if (nerve.TryGetNeuronCacheCore(in cacheKey, out var offset))
            {
                return Optional<Neuron>.From(new(offset.Value));
            }

            var localData = data;
            var cellMemory = nerve.RootNeuronWrap
                .GetUnloadedConnectionsWrap();
            var connection = cellMemory
                .FirstOrDefault(x =>
                {
                    using var neuronValue = x.NeuronWrap.Location.ReadLock();
                    return neuronValue.RefReadOnlyValue.Data == localData;
                })
                .NullWhenDefault();
            if (!connection.HasValue)
            {
                return Optional<Neuron>.Null;
            }

            using var connectionValue = connection.Value.Location.ReadLock();
            nerve.TrySetNeuronCacheCore(in cacheKey, in connectionValue.RefReadOnlyValue.Neuron.Offset);
            return Optional<Neuron>.From(connectionValue.RefReadOnlyValue.Neuron);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public Neuron FindOrAddNeuron(ref readonly TData data)
        {
            var cacheKey = nerve.CreateNeuronCacheKey(in data);
            return nerve.FindNeuronCore(ref cacheKey, in data) is { HasValue: true } optional
                ? optional.Value
                : nerve.AddNeuronCore(ref cacheKey, in data);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private Neuron AddNeuronCore(
            ref readonly NerveCacheKey cacheKey,
            ref readonly TData data)
        {
            using var @lock = nerve.RootNeuronWrap.Location.WriteLock();
            if (nerve.FindNeuronCore(in cacheKey, in data) is { HasValue: true } neuron)
            {
                return neuron.Value;
            }

            nerve.Access.Create(NeuronValue<TData>.Default with { Data = data }, out var neuronValue);
            Interlocked.Increment(ref nerve.Counter.UnsafeRefValue.NeuronCount);

            var locationWrap = nerve.RootNeuronWrap.LocationWrap;
            nerve.Access
                .Create(ConnectionValue<TLink>.Default with
                {
                    Neuron = new(neuronValue.Offset),
                    Next = @lock.RefValue.Connection,
                    NextCount = nerve.RootNeuronWrap.ConnectionWrap is { HasValue: true } connectionWrap
                            ? connectionWrap.Value.Location.ReadLock((scoped ref readonly x) => x.NextCount) + 1
                            : 0
                },
                    out var connectionValue);
            Interlocked.Increment(ref nerve.Counter.UnsafeRefValue.ConnectionCount);

            locationWrap.Location.WriteLock((scoped ref x) =>
                x.Connection = new(connectionValue.Offset));

            nerve.TrySetNeuronCacheCore(in cacheKey, in neuronValue.Offset);
            return new(neuronValue.Offset);
        }
    }
}
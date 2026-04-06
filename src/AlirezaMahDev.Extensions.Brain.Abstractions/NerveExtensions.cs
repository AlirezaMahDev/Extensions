using System.Diagnostics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public Neuron FindOrAddNeuron(ref readonly TData data)
        {
            var cacheKey = INerve<TData, TLink>.CreateNeuronCacheKey(in data);
            return nerve.FindNeuronCore(ref cacheKey, in data) is { HasValue: true } optional
                ? optional.Value
                : nerve.AddNeuronCore(ref cacheKey, in data);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public Optional<Neuron> FindNeuronCore(ref readonly NerveCacheKey cacheKey, ref readonly TData data)
        {
            if (nerve.TryGetNeuronCacheCore(in cacheKey, out var neuron))
            {
                if (neuron.Offset.IsDefault)
                    Debugger.Break();
                return Optional<Neuron>.From(neuron);
            }

            var localData = data;
            var cellMemory = nerve.RootNeuronWrap.GetUnloadedNeuronsWrap();
            var neuronWrap = cellMemory.FirstOrDefault(x =>
                    x.Location.ReadLock((scoped ref readonly value) => value.Data == localData))
                .NullWhenDefault();

            if (neuronWrap?.Location.Offset.IsDefault == true)
                Debugger.Break();

            return neuronWrap.HasValue
                ? Optional<Neuron>.From(new(neuronWrap.Value.Location.Offset))
                : Optional<Neuron>.Null;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private Neuron AddNeuronCore(
            ref readonly NerveCacheKey cacheKey,
            ref readonly TData data)
        {
            using var @lock = nerve.RootNeuronWrap.Location.WriteLock();
            if (nerve.FindNeuronCore(in cacheKey, in data) is { HasValue: true } neuron)
            {
                if (neuron.Value.Offset.IsDefault)
                    Debugger.Break();
                return neuron.Value;
            }

            nerve.Access.Create(NeuronValue<TData>.Default with
                {
                    Data = data,
                    Next = @lock.RefValue.Next,
                },
                out var neuronValue);
            Neuron newNeuron = new(neuronValue.Offset);
            @lock.RefValue.Next = newNeuron;
            nerve.GetOrAddNeuronCacheCore(in cacheKey, in newNeuron);


            if (newNeuron.Offset.IsDefault)
                Debugger.Break();

            return newNeuron;
        }
    }
}
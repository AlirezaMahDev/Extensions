using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveLearnExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public void Learn(ReadOnlyMemoryValue<TLink> link, ReadOnlyMemory<TData> data)
        {
            var connectionWrap = nerve.ConnectionWrap;
            for (var i = 0; i < data.Length; i++)
            {
                var neuron = nerve.FindOrAddNeuron(data.Span[i]);
                var connection = connectionWrap.FindOrAdd(neuron, link.Value);
                connectionWrap = connection.Wrap(nerve);
                Interlocked.Increment(ref connectionWrap.Location.RefValue.Weight);
                Interlocked.Increment(ref connectionWrap.NeuronWrap.Location.RefValue.Weight);
            }
        }

        public async ValueTask LearnAsync(ReadOnlyMemoryValue<TLink> link,
            ReadOnlyMemory<TData> data,
            CancellationToken cancellationToken = default)
        {
            var connectionWrap = nerve.ConnectionWrap;
            for (var i = 0; i < data.Length; i++)
            {
                var neuron = await nerve.FindOrAddNeuronAsync(data.Span[i], cancellationToken);
                var connection = await connectionWrap.FindOrAddAsync(neuron, link, cancellationToken);
                connectionWrap = connection.Wrap(nerve);
                Interlocked.Increment(ref connectionWrap.Location.RefValue.Weight);
                Interlocked.Increment(ref connectionWrap.NeuronWrap.Location.RefValue.Weight);
            }
        }
    }
}
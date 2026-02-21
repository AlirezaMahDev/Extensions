using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveLearnExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public async ValueTask LearnAsync(Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> data,
            CancellationToken cancellationToken = default)
        {
            var connectionWrap = nerve.ConnectionWrap;
            for (var i = 0; i < data.Length; i++)
            {
                var neuron = await nerve.FindOrAddNeuronAsync(data.ElementAt(i), cancellationToken);
                var connection = await connectionWrap.FindOrAddAsync(neuron, linkFunc(data[..i]), cancellationToken);
                connectionWrap = connection.Wrap(nerve);
                Interlocked.Increment(ref connectionWrap.Location.RefValue.Weight);
                Interlocked.Increment(ref connectionWrap.NeuronWrap.Location.RefValue.Weight);
            }
        }
    }
}
public static class NerveHotLearnExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public async ValueTask HotLearnAsync(Func<ReadOnlyMemory<TData>, TLink> linkFunc,
            ReadOnlyMemory<TData> data,
            CancellationToken cancellationToken = default)
        {
            var connectionWrap = nerve.ConnectionWrap;
            for (var i = 0; i < data.Length; i++)
            {
                var neuron = await nerve.FindOrAddNeuronAsync(data.ElementAt(i), cancellationToken);
                var connection = await connectionWrap.FindOrAddAsync(neuron, linkFunc(data[..i]), cancellationToken);
                connectionWrap = connection.Wrap(nerve);
                Interlocked.Increment(ref connectionWrap.Location.RefValue.Weight);
                Interlocked.Increment(ref connectionWrap.NeuronWrap.Location.RefValue.Weight);
            }
        }
    }
}
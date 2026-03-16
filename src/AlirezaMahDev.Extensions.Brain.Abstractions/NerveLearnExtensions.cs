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
            CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap = nerve.ConnectionWrap;
            for (int i = 0; i < data.Length; i++)
            {
                Neuron neuron = await nerve.FindOrAddNeuronAsync(data.ElementAt(i), cancellationToken);
                Connection connection =
                    await connectionWrap.FindOrAddAsync(neuron, linkFunc(data[..(i + 1)]), cancellationToken);
                connectionWrap = connection.Wrap(nerve);
                Interlocked.Increment(ref connectionWrap.Location.RefValue.Weight);
                Interlocked.Increment(ref connectionWrap.NeuronWrap.Location.RefValue.Weight);
            }
        }
    }
}
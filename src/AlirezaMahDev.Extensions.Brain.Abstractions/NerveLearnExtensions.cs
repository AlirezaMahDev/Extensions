namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveLearnExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public void Learn(Func<ReadOnlyMemory<TData>, TLink> linkFunc, ReadOnlyMemory<TData> data)
        {
            var connectionWrap = nerve.RootConnectionWrap;
            for (var i = 0; i < data.Length; i++)
            {
                var neuron = nerve.FindOrAddNeuron(in data.ElementAt(i).Value);
                connectionWrap = connectionWrap.FindOrAdd(in neuron, linkFunc(data[..i])).Wrap(nerve);
                Interlocked.Increment(ref connectionWrap.NeuronWrap.Location.RefValue.Weight);
                Interlocked.Increment(ref connectionWrap.Location.RefValue.Weight);
            }
        }
    }
}
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
                var link = linkFunc(data[..i]);
                var findOrAdd = connectionWrap.FindOrAdd(in neuron, in link);
                connectionWrap = findOrAdd.NewWrap(nerve);
                var neuronWrapLocationWrap = connectionWrap.NeuronWrap.LocationWrap;
                Interlocked.Increment(ref neuronWrapLocationWrap.Location.UnsafeRefValue.Weight);
                var connectionWrapLocationWrap = connectionWrap.LocationWrap;
                Interlocked.Increment(ref connectionWrapLocationWrap.Location.UnsafeRefValue.Weight);
            }
        }
    }
}
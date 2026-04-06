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
                var link = linkFunc(data[..i]);
                var neuron = nerve.FindOrAddNeuron(in data.ElementAt(i).Value);
                var connection = connectionWrap.FindOrAdd(in link, in neuron);
                connectionWrap = connection.NewWrap(nerve);
                connectionWrap.NeuronWrap.Wrap.Location.UnsafeAccessRef((scoped ref value) =>
                    Interlocked.Increment(ref value.Weight));
                connectionWrap.Wrap.Location.UnsafeAccessRef((scoped ref value) =>
                    Interlocked.Increment(ref value.Weight));
            }
        }
    }
}
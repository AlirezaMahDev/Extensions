namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NeuronExtensions
{
    extension<TData, TLink>(Neuron neuron)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public CellWrap<Neuron, NeuronValue<TData>, TData, TLink> Wrap(INerve<TData, TLink> nerve) =>
            new(nerve, neuron);

        public CellWrap<Neuron, NeuronValue<TData>, TData, TLink> Wrap<TCell, TValue>(
            in CellWrap<TCell, TValue, TData, TLink> wrap)
            where TCell : ICell
            where TValue : unmanaged, ICellValue<TValue> =>
            new(wrap.Nerve, neuron);
    }
}
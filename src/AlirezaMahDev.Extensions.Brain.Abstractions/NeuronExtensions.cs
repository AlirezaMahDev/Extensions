namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NeuronExtensions
{
    extension<TData, TLink>(in Neuron neuron)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public CellWrap<NeuronValue<TData>, TData, TLink> NewWrap(INerve<TData, TLink> nerve)
        {
            DataLocation<NeuronValue<TData>>.Read(nerve.Access, neuron.Offset, out var location);
            return new(nerve, location);
        }

        public CellWrap<NeuronValue<TData>, TData, TLink> NewWrap<TValue>(
            ref CellWrap<TValue, TData, TLink> wrap)
            where TValue : unmanaged, ICellValue<TValue>
        {
            DataLocation<NeuronValue<TData>>.Read(wrap.Nerve.Access, neuron.Offset, out var location);
            return new(wrap.Nerve, location);
        }
    }
}
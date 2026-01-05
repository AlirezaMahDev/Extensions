using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NeuronExtensions
{
    extension<TData, TLink>(Neuron<TData, TLink> neuron)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public NeuronWrap<TData, TLink> Wrap(INerve<TData, TLink> nerve) =>
            new(nerve, neuron);

        public NeuronWrap<TData, TLink> Wrap<TWrap>(TWrap wrap)
            where TWrap : ICellWrap<TData, TLink> =>
            new(wrap.Nerve, neuron);
    }
}
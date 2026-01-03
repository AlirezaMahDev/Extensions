using System.Collections;
using System.Numerics;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public class NeuronWrapDebugView<TData, TLink>(NeuronWrap<TData, TLink> wrap)
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public NeuronWrap<TData, TLink> Wrap { get; } = wrap;
    public IEnumerable<ConnectionWrap<TData, TLink>> ConnectionsWrap =>
        Wrap.GetConnectionsWrap();
}
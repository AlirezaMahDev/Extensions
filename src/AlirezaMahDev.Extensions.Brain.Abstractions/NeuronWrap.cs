using System.Diagnostics;
using System.Numerics;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[DebuggerTypeProxy(typeof(NeuronWrapDebugView<,>))]
public readonly record struct NeuronWrap<TData, TLink>(INerve<TData, TLink> Nerve, Neuron<TData, TLink> Cell)
    : ICellWrap<Neuron<TData, TLink>, NeuronValue<TData>, TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public override string ToString()
    {
        return $"{Cell} {RefValue}";
    }
    
    public DataWrap<NeuronValue<TData>> Location => new(Nerve.Access, new(Cell.Offset));
    public ref readonly NeuronValue<TData> RefValue => ref Location.RefValue;
    public ref readonly TData RefData => ref Location.RefValue.Data;

    public Connection<TData, TLink>? Connection =>
        RefValue.Connection.IsNull
            ? null
            : new(RefValue.Connection);
    public ConnectionWrap<TData, TLink>? ConnectionWrap =>
        Connection?.Wrap(Nerve);

    public IEnumerable<Connection<TData, TLink>> GetConnections()
    {
        var current = Connection;
        while (current.HasValue)
        {
            yield return current.Value;
            current = current.Value.Wrap(Nerve).Next;
        }
    }

    public IEnumerable<ConnectionWrap<TData, TLink>> GetConnectionsWrap()
    {
        var current = ConnectionWrap;
        while (current.HasValue)
        {
            yield return current.Value;
            current = current.Value.NextWrap;
        }
    }
}
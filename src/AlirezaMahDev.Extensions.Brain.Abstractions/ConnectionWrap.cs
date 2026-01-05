using System.Diagnostics;
using System.Numerics;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[DebuggerTypeProxy(typeof(ConnectionWrapDebugView<,>))]
public readonly record struct ConnectionWrap<TData, TLink>(
    INerve<TData, TLink> Nerve,
    Connection<TData, TLink> Cell)
    : ICellWrap<Connection<TData, TLink>, ConnectionValue<TLink>, TData, TLink>,
        IComparable<DataPairLink<TData, TLink>>
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

    public DataWrap<ConnectionValue<TLink>> Location => new(Nerve.Access, new(Cell.Offset));
    public ref readonly ConnectionValue<TLink> RefValue => ref Location.RefValue;
    public ref readonly TLink RefLink => ref Location.RefValue.Link;

    public Neuron<TData, TLink> Neuron => new(RefValue.Neuron);
    public NeuronWrap<TData, TLink> NeuronWrap => Neuron.Wrap(Nerve);

    public Connection<TData, TLink>? Previous =>
        RefValue.Previous.IsNull
            ? null
            : new(RefValue.Previous);

    public ConnectionWrap<TData, TLink>? PreviousWrap =>
        RefValue.Previous.IsNull
            ? null
            : new(Nerve, new(RefValue.Previous));

    public Connection<TData, TLink>? Next =>
        RefValue.Next.IsNull
            ? null
            : new(RefValue.Next);

    public ConnectionWrap<TData, TLink>? NextWrap =>
        RefValue.Next.IsNull
            ? null
            : new(Nerve, new(RefValue.Next));

    public Connection<TData, TLink>? Child =>
        RefValue.Child.IsNull
            ? null
            : new(RefValue.Child);

    public ConnectionWrap<TData, TLink>? ChildWrap =>
        RefValue.Child.IsNull
            ? null
            : new(Nerve, new(RefValue.Child));

    public int CompareTo(DataPairLink<TData, TLink> other)
    {
        var link = Math.Abs(Comparer<TLink>.Default.Compare(RefLink, other.Link));
        if (link != 0)
            return link;

        var data = Math.Abs(Comparer<TData>.Default.Compare(NeuronWrap.RefData, other.Data));
        if (data != 0)
            return data;

        return 0;
    }

    public int CompareTo(TLink other)
    {
        var link = Math.Abs(Comparer<TLink>.Default.Compare(RefLink, other));
        if (link != 0)
            return link;

        return 0;
    }

    public IEnumerable<Connection<TData, TLink>> GetConnections()
    {
        var current = Child;
        while (current.HasValue)
        {
            yield return current.Value;
            current = current.Value.Wrap(Nerve).Next;
        }
    }

    public IEnumerable<ConnectionWrap<TData, TLink>> GetConnectionsWrap()
    {
        var current = ChildWrap;
        while (current.HasValue)
        {
            yield return current.Value;
            current = current.Value.NextWrap;
        }
    }
}
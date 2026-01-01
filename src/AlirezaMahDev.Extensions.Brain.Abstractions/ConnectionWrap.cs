using System.Collections;
using System.Numerics;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public readonly record struct ConnectionWrap<TData, TLink>(INerve<TData, TLink> Nerve, Connection<TData, TLink> Cell)
    : ICellWrap<Connection<TData, TLink>, ConnectionValue<TLink>, TData, TLink>,
        IComparable<DataPairLink<TData, TLink>>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    public DataWrap<ConnectionValue<TLink>> Location => new(Nerve.Access, new(Cell.Offset));
    public ref readonly ConnectionValue<TLink> RefValue => ref Location.RefValue;
    public ref readonly TLink RefLink => ref Location.RefValue.Link;

    public Neuron<TData, TLink> Neuron => new(RefValue.Neuron);
    public NeuronWrap<TData, TLink> NeuronWrap => Neuron.Wrap(Nerve);

    public Connection<TData, TLink>? Previous =>
        RefValue.Previous != -1
            ? new(RefValue.Previous)
            : null;
    public ConnectionWrap<TData, TLink>? PreviousWrap =>
        RefValue.Previous != -1
            ? new(Nerve, new(RefValue.Previous))
            : null;

    public Connection<TData, TLink>? Next =>
        RefValue.Next != -1
            ? new(RefValue.Next)
            : null;

    public ConnectionWrap<TData, TLink>? NextWrap =>
        RefValue.Next != -1
            ? new(Nerve, new(RefValue.Next))
            : null;

    public Connection<TData, TLink>? SubConnection =>
        RefValue.SubConnection != -1
            ? new(RefValue.SubConnection)
            : null;

    public ConnectionWrap<TData, TLink>? SubConnectionWrap =>
        RefValue.SubConnection != -1
            ? new(Nerve, new(RefValue.SubConnection))
            : null;

    public Connection<TData, TLink>? NextSubConnection =>
        RefValue.NextSubConnection != -1
            ? new(RefValue.NextSubConnection)
            : null;

    public ConnectionWrap<TData, TLink>? NextSubConnectionWrap =>
        RefValue.NextSubConnection != -1
            ? new(Nerve, new(RefValue.NextSubConnection))
            : null;

    public int CompareTo(DataPairLink<TData, TLink> other)
    {
        var link = Math.Abs(Comparer<TLink>.Default.Compare(RefLink, other.Link));
        if (link != 0)
            return link;

        var data = Math.Abs(Comparer<TData>.Default.Compare(Neuron.Wrap(Nerve).RefData, other.Data));
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

    public IEnumerable<Connection<TData, TLink>> GetSubConnections()
    {
        var current = SubConnection;
        while (current.HasValue)
        {
            yield return current.Value;
            current = current.Value.Wrap(Nerve).NextSubConnection;
        }
    }

    public IEnumerable<ConnectionWrap<TData, TLink>> GetSubConnectionsWrap()
    {
        var current = SubConnectionWrap;
        while (current.HasValue)
        {
            yield return current.Value;
            current = current.Value.NextSubConnectionWrap;
        }
    }
}
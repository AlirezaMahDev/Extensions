using System.Collections;
using System.Numerics;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;


namespace AlirezaMahDev.Extensions.Brain;

class Connection<TData, TLink> : IConnection<TData, TLink>
    where TData : unmanaged,
    IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
    ISubtractionOperators<TData, TData, TData>
    where TLink : unmanaged,
    IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
    ISubtractionOperators<TLink, TLink, TLink>
{
    private readonly Lock _lock = new();
    protected internal readonly Neuron<TData, TLink> _neuron;
    protected internal readonly Nerve<TData, TLink> _nerve;

    private IConnection<TData, TLink>[] Cache { get; set; } = [];
    public virtual DataLocation<ConnectionValue<TLink>> Location { get; }

    public Connection(NerveArgs<TData, TLink> args)
    {
        _nerve = args.Nerve;
        Location = _nerve.Location.Access.Read<ConnectionValue<TLink>>(args.Offset);
        _neuron = _nerve.NeuronFactory.GetOrCreate(args with { Offset = RefValue.Neuron });
    }

    public long Offset => Location.Offset;
    public ref ConnectionValue<TLink> RefValue => ref Location.RefValue;

    public ref TLink RefLink => ref RefValue.Link;

    public virtual INeuron<TData, TLink> Neuron => _neuron;

    public IConnection<TData, TLink>? Previous => RefValue.Previous != -1
        ? _nerve.ConnectionFactory.GetOrCreate(new NerveArgs<TData, TLink>(_nerve, RefValue.Previous))
        : null;

    public IConnection<TData, TLink>? Next => RefValue.Connection != -1
        ? _nerve.ConnectionFactory.GetOrCreate(new NerveArgs<TData, TLink>(_nerve, RefValue.Connection))
        : null;

    public virtual IConnection<TData, TLink>[] ToArray()
    {
        long? cacheKey = Cache.Length > 0 ? Cache[0].Offset : null;
        if (!cacheKey.HasValue || cacheKey.Value != Neuron.RefValue.Connection)
        {
            using var scope = _lock.EnterScope();
            var refValueConnection = Neuron.RefValue.Connection;
            if (!cacheKey.HasValue || cacheKey.Value != refValueConnection)
            {
                Cache =
                [
                    .. Neuron.GetConnections()
                        .Where(x => x.RefValue.Previous == Offset)
                        .Select(x => _nerve.ConnectionFactory
                            .GetOrCreate(new NerveArgs<TData, TLink>(_nerve, x.Offset)))
                ];
            }
        }

        return Cache;
    }

    public virtual IEnumerator<IConnection<TData, TLink>> GetEnumerator() =>
        ToArray().AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int CompareTo(DataPairLink<TData, TLink> other)
    {
        var link = Math.Abs(Comparer<TLink>.Default.Compare(RefLink, other.Link));
        if (link != 0)
            return link;

        var data = Math.Abs(Comparer<TData>.Default.Compare(Neuron.RefData, other.Data));
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
}
using System.Collections;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;


namespace AlirezaMahDev.Extensions.Brain;

class Connection<TData,TLink> : IConnection<TData,TLink>
    where TData : unmanaged
    where TLink : unmanaged
{
    private readonly Lock _lock = new();
    private readonly NerveArgs<TData,TLink> _args;

    private IConnection<TData,TLink>[] Cache { get; set; } = [];
    public virtual DataLocation<ConnectionValue<TLink>> Location { get; }

    public Connection(NerveArgs<TData,TLink> args)
    {
        _args = args;
        Location = args.Nerve.Location.Access.Read<ConnectionValue<TLink>>(args.Offset);
        Neuron = args.Nerve.NeuronFactory.GetOrCreate(args with { Offset = RefValue.Neuron });
    }

    public long Offset => _args.Offset;
    public ref ConnectionValue<TLink> RefValue => ref Location.RefValue;

    public ref TLink RefLink => ref RefValue.Link;

    public virtual INeuron<TData,TLink> Neuron { get; }

    public IConnection<TData,TLink>? Previous => RefValue.Previous != -1
        ? _args.Nerve.ConnectionFactory.GetOrCreate(_args with { Offset = RefValue.Previous })
        : null;

    public IConnection<TData,TLink>? Next => RefValue.Next != -1
        ? _args.Nerve.ConnectionFactory.GetOrCreate(_args with { Offset = RefValue.Next })
        : null;

    public virtual IConnection<TData,TLink>[] ToArray()
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
                    .. Neuron.Where(x => x.RefValue.Previous == Offset)
                        .Select(x => _args with { Offset = x.Offset })
                        .Select(x => _args.Nerve.ConnectionFactory.GetOrCreate(x))
                ];
            }
        }

        return Cache;
    }

    public virtual IEnumerator<IConnection<TData,TLink>> GetEnumerator() =>
        ToArray().AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
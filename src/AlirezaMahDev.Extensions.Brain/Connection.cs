using System.Collections;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;


namespace AlirezaMahDev.Extensions.Brain;

class Connection<TData> : IConnection<TData>
    where TData : unmanaged
{
    private readonly Lock _lock = new();
    private readonly NerveArgs<TData> _args;

    private IConnection<TData>[] Cache { get; set; } = [];
    public virtual DataLocation<ConnectionValue> Location { get; }

    public Connection(NerveArgs<TData> args)
    {
        _args = args;
        Location = args.Nerve.Location.Access.Read<ConnectionValue>(args.Offset);
        Neuron = args.Nerve.NeuronFactory.GetOrCreate(args with { Offset = RefValue.Neuron });
    }

    public long Offset => _args.Offset;
    public ref ConnectionValue RefValue => ref Location.RefValue;

    public virtual INeuron<TData> Neuron { get; }

    public IConnection<TData>? Previous => RefValue.Previous != -1
        ? _args.Nerve.ConnectionFactory.GetOrCreate(_args with { Offset = RefValue.Previous })
        : null;

    public IConnection<TData>? Next => RefValue.Next != -1
        ? _args.Nerve.ConnectionFactory.GetOrCreate(_args with { Offset = RefValue.Next })
        : null;

    public virtual IConnection<TData>[] ToArray()
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

    public virtual IEnumerator<IConnection<TData>> GetEnumerator() =>
        ToArray().AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
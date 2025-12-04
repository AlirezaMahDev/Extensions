using System.Collections;

using Microsoft.Extensions.DependencyInjection;


namespace AlirezaMahDev.Extensions.Brain;

public class Connection<TData> : IEnumerable<Connection<TData>>
    where TData : unmanaged
{
    private readonly Lock _lock = new();
    private readonly NerveArgs<TData> _args;

    protected virtual Connection<TData>[] Cache { get; set; } = [];
    protected virtual StackItem<ConnectionValue> ConnectionItem { get; }

    protected Connection(NerveArgs<TData> args, Neuron<TData> neuron, StackItem<ConnectionValue> connectionItem)
    {
        _args = args;
        ConnectionItem = connectionItem;
        Neuron = neuron;
    }

    [ActivatorUtilitiesConstructor]
    public Connection(NerveArgs<TData> args)
    {
        _args = args;
        ConnectionItem = args.Nerve.ConnectionFactory.ConnectionStack.Items.GetOrAdd(args.Id);
        Neuron = args.Nerve.NeuronFactory.GetOrCreate(args with { Id = RefValue.Neuron });
    }

    public int Id => _args.Id;
    public ref ConnectionValue RefValue => ref ConnectionItem.RefValue;

    public virtual Neuron<TData> Neuron { get; }

    public Connection<TData>? Previous => RefValue.Previous.IsNotDefault
        ? _args.Nerve.ConnectionFactory.GetOrCreate(_args with { Id = RefValue.Previous })
        : null;

    public Connection<TData>? Next => RefValue.Next.IsNotDefault
        ? _args.Nerve.ConnectionFactory.GetOrCreate(_args with { Id = RefValue.Next })
        : null;

    public virtual Connection<TData>[] ToArray()
    {
        int? cacheKey = Cache.Length > 0 ? Cache[0].Id : null;
        if (!cacheKey.HasValue || cacheKey.Value != Neuron.RefValue.Connection)
        {
            using var scope = _lock.EnterScope();
            var refValueConnection = Neuron.RefValue.Connection;
            if (!cacheKey.HasValue || cacheKey.Value != refValueConnection)
            {
                Cache =
                [
                    .. Neuron.Where(x => x.RefValue.Previous == Id)
                        .Select(x => _args with { Id = x.Id })
                        .Select(x => _args.Nerve.ConnectionFactory.GetOrCreate(x))
                ];
            }
        }

        return Cache;
    }

    public virtual IEnumerator<Connection<TData>> GetEnumerator() =>
        ToArray().AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
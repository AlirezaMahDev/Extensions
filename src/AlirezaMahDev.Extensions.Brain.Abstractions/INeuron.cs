namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INeuron<TData, TLink> : IEnumerable<IConnection<TData, TLink>>
    where TData : unmanaged
    where TLink : unmanaged
{
    long Offset { get; }
    ref NeuronValue<TData> RefValue { get; }
    ref TData RefData { get; }

    IConnection<TData, TLink>? Connection { get; }
    IConnection<TData, TLink> GetOrAdd(TData data, TLink link, IConnection<TData, TLink>? connection);

    // ValueTask<IConnection<TData, TLink>> GetOrAddAsync(TData data,
    //     TLink link,
    //     IConnection<TData, TLink>? connection,
    //     CancellationToken cancellationToken = default);
}
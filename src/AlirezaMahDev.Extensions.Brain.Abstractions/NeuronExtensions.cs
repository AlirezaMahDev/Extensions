using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

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

    extension<TData, TLink>(NeuronWrap<TData, TLink> wrap)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public void Lock(DataLocationAction<NeuronValue<TData>> action) =>
            wrap.Location.Lock(action);

        public TResult Lock<TResult>(DataLocationFunc<NeuronValue<TData>, TResult> func) =>
            wrap.Location.Lock(func);

        public async ValueTask LockAsync(DataLocationAction<NeuronValue<TData>> action,
            CancellationToken cancellationToken = default) =>
            await wrap.Location.LockAsync(action, cancellationToken);

        public async ValueTask LockAsync(DataLocationAsyncAction<NeuronValue<TData>> action,
            CancellationToken cancellationToken = default) =>
            await wrap.Location.LockAsync(action, cancellationToken);

        public async ValueTask<TResult> LockAsync<TResult>(DataLocationFunc<NeuronValue<TData>, TResult> func,
            CancellationToken cancellationToken = default) =>
            await wrap.Location.LockAsync(func, cancellationToken);

        public async ValueTask<TResult> LockAsync<TResult>(DataLocationAsyncFunc<NeuronValue<TData>, TResult> func,
            CancellationToken cancellationToken = default) =>
            await wrap.Location.LockAsync(func, cancellationToken);

        public Connection<TData, TLink>? Find(ReadOnlyMemoryValue<TData> data,
            ReadOnlyMemoryValue<TLink> link,
            Connection<TData, TLink>? previous)
        {
            var neuron = wrap.Nerve.FindOrAdd(data);
            ReadOnlyMemoryValue<NerveCacheKey> cacheKey = NerveHelper.CreateCacheKey(wrap.Cell, neuron, link, previous);
            return wrap.FindCore(cacheKey, neuron, link, previous);
        }

        private Connection<TData, TLink>? FindCore(ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
            Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link,
            Connection<TData, TLink>? previous)
        {
            if (wrap.Nerve.Cache.ConnectionSection.TryGet(in cacheKey.Value, out var connectionOffset))
                return new(connectionOffset.Value);

            Connection<TData, TLink>? result;
            if (previous is not null)
            {
                result = previous.Value.Wrap(wrap).Find(neuron, link);
            }
            else
            {
                result = wrap.GetUnloadedConnections()
                    .Select(x => x.Wrap(wrap))
                    .FirstOrDefault(x =>
                        x.RefValue.Neuron == neuron.Offset &&
                        x.RefLink.Equals(link.Value) &&
                        x.RefValue.Previous == -1)
                    .NullWhenDefault()
                    ?.Cell;

                if (result is not null)
                    wrap.Nerve.Cache.ConnectionSection.Set(in cacheKey.Value, result.Value.Offset);
            }

            return result;
        }

        public Connection<TData, TLink> FindOrAdd(ReadOnlyMemoryValue<TData> data,
            ReadOnlyMemoryValue<TLink> link,
            Connection<TData, TLink>? previous)
        {
            var neuron = wrap.Nerve.FindOrAdd(data);
            ReadOnlyMemoryValue<NerveCacheKey> cacheKey = NerveHelper.CreateCacheKey(wrap.Cell, neuron, link, previous);
            return wrap.FindCore(cacheKey, neuron, link, previous) ?? wrap.AddCore(cacheKey, neuron, link, previous);
        }

        public async ValueTask<Connection<TData, TLink>> FindOrAddAsync(ReadOnlyMemoryValue<TData> data,
            ReadOnlyMemoryValue<TLink> link,
            Connection<TData, TLink>? previous,
            CancellationToken cancellationToken = default)
        {
            var neuron = await wrap.Nerve.FindOrAddAsync(data, cancellationToken);
            ReadOnlyMemoryValue<NerveCacheKey> cacheKey = NerveHelper.CreateCacheKey(wrap.Cell, neuron, link, previous);
            return wrap.FindCore(cacheKey, neuron, link, previous) ??
                   await wrap.AddAsyncCore(cacheKey, neuron, link, previous, cancellationToken);
        }


        private Connection<TData, TLink> AddCore(ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
            Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link,
            Connection<TData, TLink>? previous)
        {
            if (previous.HasValue)
            {
                return previous.Value.Wrap(wrap).FindOrAdd(neuron, link);
            }

            return wrap.Lock(neuronDataLocation =>
            {
                var neuronDataLocationWrap = neuronDataLocation.Wrap(wrap.Nerve.Access);
                if (wrap.FindCore(cacheKey, neuron, link, null) is { } connection)
                {
                    return connection;
                }

                var connectionValue = wrap.Nerve.Access
                    .Create(ConnectionValue<TLink>.Default with
                    {
                        Neuron = neuron.Offset,
                        Next = neuronDataLocationWrap.RefValue.Connection,
                        Link = link.Value,
                    });

                neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;

                return new(wrap.Nerve.Cache.ConnectionSection.Set(
                    in cacheKey.Value,
                    connectionValue.Offset));
            });
        }


        private async ValueTask<Connection<TData, TLink>> AddAsyncCore(
            ReadOnlyMemoryValue<NerveCacheKey> cacheKey,
            Neuron<TData, TLink> neuron,
            ReadOnlyMemoryValue<TLink> link,
            Connection<TData, TLink>? previous,
            CancellationToken cancellationToken = default)
        {
            if (previous.HasValue)
            {
                return await previous.Value.Wrap(wrap).FindOrAddAsync(neuron, link, cancellationToken);
            }

            return await wrap.LockAsync(neuronDataLocation =>
                {
                    if (wrap.FindCore(cacheKey, neuron, link, null) is { } connection)
                        return connection;

                    var neuronDataLocationWrap = neuronDataLocation.Wrap(wrap.Nerve.Access);
                    var connectionValue = wrap.Nerve.Access
                        .Create(ConnectionValue<TLink>.Default with
                        {
                            Neuron = neuron.Offset,
                            Next = neuronDataLocationWrap.RefValue.Connection,
                            Link = link.Value,
                        });

                    neuronDataLocationWrap.RefValue.Connection = connectionValue.Offset;

                    return new(
                        wrap.Nerve.Cache.ConnectionSection.Set(
                            cacheKey.Value,
                            connectionValue.Offset)
                    );
                },
                cancellationToken);
        }

        public long? LastLoadedConnection
        {
            get => wrap.Nerve.Cache.LastLoadedConnection.GetOrNull(wrap.Cell.Offset);
            set => wrap.Nerve.Cache.LastLoadedConnection.Set(wrap.Cell.Offset, value ?? -1);
        }

        public IEnumerable<Connection<TData, TLink>> GetUnloadedConnections()
        {
            var unloaded = wrap.LastLoadedConnection;
            if (unloaded == -1)
                yield break;

            var connection = unloaded.HasValue ? new Connection<TData, TLink>(unloaded.Value) : wrap.Connection;
            while (connection.HasValue)
            {
                var connectionWrap = connection.Value.Wrap(wrap);
                wrap.Nerve.Cache.LastLoadedConnection.Set(NerveHelper.CreateCacheKey(wrap.Cell,
                            connectionWrap.Neuron,
                            connectionWrap.RefLink,
                            connectionWrap.Previous)
                        .Value,
                    connectionWrap.Cell.Offset);
                yield return connectionWrap.Cell;
                connection = connectionWrap.Next;
            }
        }
    }
}
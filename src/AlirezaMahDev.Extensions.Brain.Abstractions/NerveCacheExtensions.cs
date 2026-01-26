using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveCacheExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public INerveCacheSection NeuronSectionCache => nerve.Cache.GetOrAdd("n");
        public INerveCacheSection ConnectionSectionCache => nerve.Cache.GetOrAdd("c");
    }

    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public bool TryGetNeuronCache(in TData data, [NotNullWhen(true)] out Neuron? neuron)
        {
            if (nerve.TryGetNeuronCacheCore(nerve.CreateNeuronCacheKey(in data), out var offset))
            {
                neuron = new(offset.Value);
                return true;
            }

            neuron = null;
            return false;
        }

        public bool TryGetNeuronCacheCore(in NerveCacheKey cacheKey, [NotNullWhen(true)] out DataOffset? offset)
        {
            return nerve.NeuronSectionCache.TryGet(in cacheKey, out offset);
        }

        public void TrySetNeuronCache(in CellWrap<Neuron, NeuronValue<TData>, TData, TLink> neuronWrap)
        {
            nerve.TrySetNeuronCacheCore(nerve.CreateNeuronCacheKey(in neuronWrap), neuronWrap.Cell.Offset);
        }

        public void TrySetNeuronCacheCore(in NerveCacheKey cacheKey, in DataOffset offset)
        {
            nerve.NeuronSectionCache.GetOrAdd(in cacheKey, offset);
        }

        public void SetNeuronCache(in CellWrap<Neuron, NeuronValue<TData>, TData, TLink> neuronWrap)
        {
            nerve.SetNeuronCacheCore(
                nerve.CreateNeuronCacheKey(in neuronWrap),
                neuronWrap.Cell.Offset);
        }

        public void SetNeuronCacheCore(in NerveCacheKey cacheKey, in DataOffset offset)
        {
            nerve.NeuronSectionCache.Set(in cacheKey, offset);
        }

        public NerveCacheKey CreateNeuronCacheKey(in CellWrap<Neuron, NeuronValue<TData>, TData, TLink> neuronWrap) =>
            nerve.CreateNeuronCacheKey(in neuronWrap.RefData);

        public NerveCacheKey CreateNeuronCacheKey(in TData data) =>
            NerveCacheKey.Create(in data);
    }

    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public bool TryGetNeuronConnectionCacheCore(in NerveCacheKey cacheKey,
            [NotNullWhen(true)] out DataOffset? offset)
        {
            return nerve.ConnectionSectionCache.TryGet(in cacheKey, out offset);
        }

        public void TrySetNeuronConnectionCache(in Neuron from,
            in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            nerve.TrySetNeuronConnectionCacheCore(
                nerve.CreateNeuronConnectionCacheKey(in from, in connectionWrap),
                connectionWrap.Cell.Offset);
        }

        public void TrySetNeuronConnectionCacheCore(in NerveCacheKey cacheKey, in DataOffset offset)
        {
            nerve.ConnectionSectionCache.GetOrAdd(in cacheKey, offset);
        }

        public void SetNeuronConnectionCache(in Neuron from,
            in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            nerve.SetNeuronConnectionCacheCore(
                nerve.CreateNeuronConnectionCacheKey(in from, in connectionWrap),
                connectionWrap.Cell.Offset);
        }

        public void SetNeuronConnectionCacheCore(in NerveCacheKey cacheKey, in DataOffset offset)
        {
            nerve.ConnectionSectionCache.Set(in cacheKey, offset);
        }

        public NerveCacheKey CreateNeuronConnectionCacheKey(in Neuron from,
            in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap) =>
            nerve.CreateNeuronConnectionCacheKey(
                in from,
                connectionWrap.Neuron,
                in connectionWrap.RefLink);

        public NerveCacheKey CreateNeuronConnectionCacheKey(in Neuron from,
            in Neuron to,
            in TLink link) =>
            NerveCacheKey.Create(
                from.Offset,
                to.Offset,
                in link);
    }

    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public bool TryGetConnectionCache(
            in Neuron from,
            in Neuron to,
            in TLink link,
            in Connection previous,
            [NotNullWhen(true)] out Connection? connection)
        {
            if (nerve.TryGetConnectionCacheCore(
                    nerve.CreateConnectionCacheKey(in from, in to, in link, in previous),
                    offset: out var offset))
            {
                connection = new(offset.Value);
                return true;
            }

            connection = null;
            return false;
        }

        public bool TryGetConnectionCacheCore(in NerveCacheKey cacheKey, [NotNullWhen(true)] out DataOffset? offset)
        {
            return nerve.ConnectionSectionCache.TryGet(in cacheKey, out offset);
        }

        public void TrySetConnectionCache(in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            nerve.TrySetConnectionCacheCore(nerve.CreateConnectionCacheKey(in connectionWrap),
                connectionWrap.Cell.Offset);
        }

        public void TrySetConnectionCacheCore(in NerveCacheKey cacheKey, in DataOffset offset)
        {
            nerve.ConnectionSectionCache.GetOrAdd(in cacheKey, offset);
        }

        public void SetConnectionCache(in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap)
        {
            nerve.SetConnectionCacheCore(
                nerve.CreateConnectionCacheKey(in connectionWrap),
                connectionWrap.Cell.Offset);
        }

        public void SetConnectionCacheCore(in NerveCacheKey cacheKey, in DataOffset offset)
        {
            nerve.ConnectionSectionCache.Set(in cacheKey, offset);
        }

        public NerveCacheKey CreateConnectionCacheKey(
            in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap) =>
            nerve.CreateConnectionCacheKey(
                connectionWrap.PreviousWrap!.Value.Neuron,
                connectionWrap.Neuron,
                in connectionWrap.RefLink,
                connectionWrap.Previous!.Value);

        public NerveCacheKey CreateConnectionCacheKey(
            in CellWrap<Connection, ConnectionValue<TLink>, TData, TLink> connectionWrap,
            in Neuron to,
            in TLink link) =>
            nerve.CreateConnectionCacheKey(
                connectionWrap.Neuron,
                to,
                in link,
                connectionWrap.Cell);

        public NerveCacheKey CreateConnectionCacheKey(
            in Neuron from,
            in Neuron to,
            in TLink link,
            in Connection previous) =>
            NerveCacheKey.Create(from.Offset, to.Offset, in link, previous.Offset);
    }
}
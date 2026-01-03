using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using AlirezaMahDev.Extensions.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveCacheExtensions
{
    extension<TData, TLink>(INerveCache<TData, TLink> nerveCache)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public INerveCacheSection NeuronSection => nerveCache.GetOrAdd("n");
        public INerveCacheSection ConnectionSection => nerveCache.GetOrAdd("c");
        public INerveCacheSection LastLoadedConnection => nerveCache.GetOrAdd("l");
    }

    extension<TData, TLink>(INerveCache<TData, TLink> nerveCache)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public bool TryGetNeuronCache(in TData data, [NotNullWhen(true)] out Neuron<TData, TLink>? neuron)
        {
            if (nerveCache.TryGetNeuronCacheCore(nerveCache.CreateNeuronCacheKey(in data), out var offset))
            {
                neuron = new(offset.Value);
                return true;
            }

            neuron = null;
            return false;
        }

        public bool TryGetNeuronCacheCore(in NerveCacheKey cacheKey, [NotNullWhen(true)] out long? offset)
        {
            return nerveCache.NeuronSection.TryGet(in cacheKey, out offset);
        }

        public void TrySetNeuronCache(in NeuronWrap<TData, TLink> neuronWrap)
        {
            nerveCache.TrySetNeuronCacheCore(nerveCache.CreateNeuronCacheKey(in neuronWrap), neuronWrap.Cell.RefOffset);
        }

        public void TrySetNeuronCacheCore(in NerveCacheKey cacheKey, in long offset)
        {
            nerveCache.NeuronSection.GetOrAdd(in cacheKey, offset);
        }

        public void SetNeuronCache(in NeuronWrap<TData, TLink> neuronWrap)
        {
            nerveCache.SetNeuronCacheCore(
                nerveCache.CreateNeuronCacheKey(in neuronWrap),
                neuronWrap.Cell.Offset);
        }

        public void SetNeuronCacheCore(in NerveCacheKey cacheKey, in long offset)
        {
            nerveCache.NeuronSection.Set(in cacheKey, offset);
        }

        public NerveCacheKey CreateNeuronCacheKey(in NeuronWrap<TData, TLink> neuronWrap) =>
            nerveCache.CreateNeuronCacheKey(in neuronWrap.RefData);

        public NerveCacheKey CreateNeuronCacheKey(in TData data) =>
            NerveCacheKey.Create(in data);
    }

    extension<TData, TLink>(INerveCache<TData, TLink> nerveCache)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public bool TryGetNeuronConnectionCacheCore(in NerveCacheKey cacheKey, [NotNullWhen(true)] out long? offset)
        {
            return nerveCache.ConnectionSection.TryGet(in cacheKey, out offset);
        }

        public void TrySetNeuronConnectionCache(in Neuron<TData, TLink> from,
            in ConnectionWrap<TData, TLink> connectionWrap)
        {
            nerveCache.TrySetNeuronConnectionCacheCore(
                nerveCache.CreateNeuronConnectionCacheKey(in from, in connectionWrap),
                connectionWrap.Cell.Offset);
        }

        public void TrySetNeuronConnectionCacheCore(in NerveCacheKey cacheKey, in long offset)
        {
            nerveCache.ConnectionSection.GetOrAdd(in cacheKey, offset);
        }

        public void SetNeuronConnectionCache(in Neuron<TData, TLink> from,
            in ConnectionWrap<TData, TLink> connectionWrap)
        {
            nerveCache.SetNeuronConnectionCacheCore(
                nerveCache.CreateNeuronConnectionCacheKey(in from, in connectionWrap),
                connectionWrap.Cell.Offset);
        }

        public void SetNeuronConnectionCacheCore(in NerveCacheKey cacheKey, in long offset)
        {
            nerveCache.ConnectionSection.Set(in cacheKey, offset);
        }

        public NerveCacheKey CreateNeuronConnectionCacheKey(in Neuron<TData, TLink> from,
            in ConnectionWrap<TData, TLink> connectionWrap) =>
            nerveCache.CreateNeuronConnectionCacheKey(
                in from,
                connectionWrap.Neuron,
                in connectionWrap.RefLink);

        public NerveCacheKey CreateNeuronConnectionCacheKey(in Neuron<TData, TLink> from,
            in Neuron<TData, TLink> to,
            in TLink link) =>
            NerveCacheKey.Create(
                from.Offset,
                to.Offset,
                in link);
    }

    extension<TData, TLink>(INerveCache<TData, TLink> nerveCache)
        where TData : unmanaged,
        IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
        ISubtractionOperators<TData, TData, TData>
        where TLink : unmanaged,
        IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
        ISubtractionOperators<TLink, TLink, TLink>
    {
        public bool TryGetConnectionCache(
            in Neuron<TData, TLink> from,
            in Neuron<TData, TLink> to,
            in TLink link,
            in Connection<TData, TLink> previous,
            [NotNullWhen(true)] out Connection<TData, TLink>? connection)
        {
            if (nerveCache.TryGetConnectionCacheCore(
                    nerveCache.CreateConnectionCacheKey(in from, in to, in link, in previous),
                    offset: out var offset))
            {
                connection = new(offset.Value);
                return true;
            }

            connection = null;
            return false;
        }

        public bool TryGetConnectionCacheCore(in NerveCacheKey cacheKey, [NotNullWhen(true)] out long? offset)
        {
            return nerveCache.ConnectionSection.TryGet(in cacheKey, out offset);
        }

        public void TrySetConnectionCache(in ConnectionWrap<TData, TLink> connectionWrap)
        {
            nerveCache.TrySetConnectionCacheCore(nerveCache.CreateConnectionCacheKey(in connectionWrap),
                connectionWrap.Cell.Offset);
        }

        public void TrySetConnectionCacheCore(in NerveCacheKey cacheKey, in long offset)
        {
            nerveCache.ConnectionSection.GetOrAdd(in cacheKey, offset);
        }

        public void SetConnectionCache(in ConnectionWrap<TData, TLink> connectionWrap)
        {
            nerveCache.SetConnectionCacheCore(
                nerveCache.CreateConnectionCacheKey(in connectionWrap),
                connectionWrap.Cell.Offset);
        }

        public void SetConnectionCacheCore(in NerveCacheKey cacheKey, in long offset)
        {
            nerveCache.ConnectionSection.Set(in cacheKey, offset);
        }

        public NerveCacheKey CreateConnectionCacheKey(in ConnectionWrap<TData, TLink> connectionWrap) =>
            nerveCache.CreateConnectionCacheKey(
                connectionWrap.PreviousWrap!.Value.Neuron,
                connectionWrap.Neuron,
                in connectionWrap.RefLink,
                connectionWrap.Previous!.Value);

        public NerveCacheKey CreateConnectionCacheKey(in ConnectionWrap<TData, TLink> connectionWrap,
            in Neuron<TData, TLink> to,
            in TLink link) =>
            nerveCache.CreateConnectionCacheKey(
                connectionWrap.Neuron,
                to,
                in link,
                connectionWrap.Cell);

        public NerveCacheKey CreateConnectionCacheKey(
            in Neuron<TData, TLink> from,
            in Neuron<TData, TLink> to,
            in TLink link,
            in Connection<TData, TLink> previous) =>
            NerveCacheKey.Create(in from.RefOffset, in to.RefOffset, in link, previous.Offset);
    }
}
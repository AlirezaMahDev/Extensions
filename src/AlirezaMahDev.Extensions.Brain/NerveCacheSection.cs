using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.Brain.Abstractions;
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

internal class NerveCacheSection : INerveCacheSection
{
    private readonly ConcurrentDictionary<UInt128, DataOffset> _cache = [];

    private static UInt128 GenerateHash<TKey>(in TKey key)
        where TKey : unmanaged
    {
        return NerveCacheKey.Create(in key).Hash;
    }

    public bool TryGet<TKey>(in TKey key, [NotNullWhen(true)] out DataOffset? value)
        where TKey : unmanaged
    {
        return TryGetCore(GenerateHash(in key), out value);
    }

    public bool TryGet(in NerveCacheKey key, [NotNullWhen(true)] out DataOffset? value)
    {
        return TryGetCore(key.Hash, out value);
    }

    private bool TryGetCore(in UInt128 key, [NotNullWhen(true)] out DataOffset? value)
    {
        return (value = GetOrDefaultCore(key)) is not null;
    }

    public DataOffset? GetOrNull<TKey>(in TKey key)
        where TKey : unmanaged
    {
        return GetOrDefaultCore(GenerateHash(in key)) is { IsDefault: false } value ? value : null;
    }

    public DataOffset? GetOrNull(in NerveCacheKey key)
    {
        return GetOrDefaultCore(key.Hash);
    }

    private DataOffset? GetOrDefaultCore(in UInt128 key)
    {
        return _cache.TryGetValue(key, out DataOffset result) ? result : null;
    }

    public DataOffset Set<TKey>(in TKey key, DataOffset value)
        where TKey : unmanaged
    {
        return SetCore(GenerateHash(in key), value);
    }

    public DataOffset Set(in NerveCacheKey key, DataOffset value)
    {
        return SetCore(key.Hash, value);
    }

    private DataOffset SetCore(in UInt128 key, DataOffset value)
    {
        return _cache[key] = value;
    }

    public DataOffset GetOrAdd<TKey>(in TKey key, Func<UInt128, DataOffset> factory)
        where TKey : unmanaged
    {
        return GetOrAddCore(GenerateHash(in key), factory);
    }

    public DataOffset GetOrAdd(in NerveCacheKey key, Func<UInt128, DataOffset> factory)
    {
        return GetOrAddCore(key.Hash, factory);
    }

    public DataOffset GetOrAdd<TKey>(in TKey key, DataOffset value)
        where TKey : unmanaged
    {
        return GetOrAddCore(GenerateHash(in key), value);
    }

    public DataOffset GetOrAdd(in NerveCacheKey key, DataOffset value)
    {
        return GetOrAddCore(key.Hash, value);
    }

    private DataOffset GetOrAddCore(in UInt128 key, Func<UInt128, DataOffset> factory)
    {
        return _cache.GetOrAdd(key, factory);
    }

    private DataOffset GetOrAddCore(in UInt128 key, DataOffset value)
    {
        return _cache.GetOrAdd(key, value);
    }
}
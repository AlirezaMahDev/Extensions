using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.Brain.Abstractions;

namespace AlirezaMahDev.Extensions.Brain;

class NerveCacheSection : INerveCacheSection
{
    private readonly ConcurrentDictionary<UInt128, long> _cache = [];

    private static UInt128 GenerateHash<TKey>(in TKey key)
        where TKey : unmanaged => NerveCacheKey.Create(in key).Hash;

    public bool TryGet<TKey>(in TKey key, [NotNullWhen(true)] out long? value)
        where TKey : unmanaged =>
        TryGetCore(GenerateHash(in key), out value);

    public bool TryGet(in NerveCacheKey key, [NotNullWhen(true)] out long? value) =>
        TryGetCore(key.Hash, out value);

    private bool TryGetCore(in UInt128 key, [NotNullWhen(true)] out long? value) =>
        (value = GetOrDefaultCore(key)) is not null;

    public long? GetOrNull<TKey>(in TKey key)
        where TKey : unmanaged =>
        GetOrDefaultCore(GenerateHash(in key)) is { } value && value != 0 ? value : null;

    public long? GetOrNull(in NerveCacheKey key) =>
        GetOrDefaultCore(key.Hash);

    private long? GetOrDefaultCore(in UInt128 key) =>
        _cache.TryGetValue(key, out var result) ? result : null;

    public long Set<TKey>(in TKey key, long value)
        where TKey : unmanaged =>
        SetCore(GenerateHash(in key), value);

    public long Set(in NerveCacheKey key, long value) =>
        SetCore(key.Hash, value);

    private long SetCore(in UInt128 key, long value) =>
        _cache[key] = value;

    public long GetOrAdd<TKey>(in TKey key, Func<UInt128, long> factory)
        where TKey : unmanaged =>
        GetOrAddCore(GenerateHash(in key), factory);

    public long GetOrAdd(in NerveCacheKey key, Func<UInt128, long> factory) =>
        GetOrAddCore(key.Hash, factory);

    public long GetOrAdd<TKey>(in TKey key, long value)
        where TKey : unmanaged =>
        GetOrAddCore(GenerateHash(in key), value);
    public long GetOrAdd(in NerveCacheKey key, long value) =>
        GetOrAddCore(key.Hash, value);

    private long GetOrAddCore(in UInt128 key, Func<UInt128, long> factory) =>
        _cache.GetOrAdd(key, factory);
    private long GetOrAddCore(in UInt128 key, long value) =>
        _cache.GetOrAdd(key, value);
}
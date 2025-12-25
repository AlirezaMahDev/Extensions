using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO.Hashing;
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain;

class NerveCache()
{
    private readonly ConcurrentDictionary<UInt128, object> _cache = [];

    private static UInt128 GenerateHash<TKey>(in TKey key)
        where TKey : unmanaged =>
            XxHash128.HashToUInt128(
                MemoryMarshal.AsBytes(
                        MemoryMarshal.CreateReadOnlySpan(in key, 1)));

    public bool TryGet<TKey, TValue>(in TKey key, [NotNullWhen(true)] out TValue? value)
        where TKey : unmanaged
        where TValue : class =>
            TryGetCore(GenerateHash(in key), out value);

    public bool TryGet<TValue>(in NerveCacheKey key, [NotNullWhen(true)] out TValue? value)
        where TValue : class =>
            TryGetCore(key.Hash, out value);

    private bool TryGetCore<TValue>(in UInt128 key, [NotNullWhen(true)] out TValue? value)
        where TValue : class =>
            (value = GetOrDefaultCore<TValue>(key)) is not null;

    public TValue? GetOrDefault<TKey, TValue>(in TKey key)
        where TKey : unmanaged
        where TValue : class =>
            GetOrDefaultCore<TValue>(GenerateHash(in key));
    
    public TValue? GetOrDefault<TValue>(in NerveCacheKey key)
        where TValue : class =>
            GetOrDefaultCore<TValue>(key.Hash);

    private TValue? GetOrDefaultCore<TValue>(in UInt128 key)
        where TValue : class =>
            _cache.GetValueOrDefault(key) as TValue;

    public TValue Set<TKey, TValue>(in TKey key, TValue value)
        where TKey : unmanaged
        where TValue : class =>
            SetCore(GenerateHash(in key), value);
    
    public TValue Set<TValue>(in NerveCacheKey key, TValue value)
        where TValue : class =>
            SetCore(key.Hash, value);

    private TValue SetCore<TValue>(in UInt128 key, TValue value)
        where TValue : class =>
            (TValue)(_cache[key] = value);

    public TValue GetOrAdd<TKey, TValue>(in TKey key, Func<TValue> factory)
        where TKey : unmanaged
        where TValue : class =>
            GetOrAddCore(GenerateHash(in key), factory);

    public TValue GetOrAdd<TValue>(in NerveCacheKey key, Func<TValue> factory)
        where TValue : class =>
            GetOrAddCore(key.Hash, factory);

    private TValue GetOrAddCore<TValue>(in UInt128 key, Func<TValue> factory)
        where TValue : class =>
            (TValue)_cache.GetOrAdd(key, factory);
}

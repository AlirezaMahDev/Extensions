using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerveCacheSection
{
    bool TryGet(in NerveCacheKey key, [NotNullWhen(true)] out long? value);

    bool TryGet<TKey>(in TKey key, [NotNullWhen(true)] out long? value)
        where TKey : unmanaged;

    long? GetOrNull(in NerveCacheKey key);

    long? GetOrNull<TKey>(in TKey key)
        where TKey : unmanaged;

    long Set(in NerveCacheKey key, long value);

    long Set<TKey>(in TKey key, long value)
        where TKey : unmanaged;

    long GetOrAdd(in NerveCacheKey key, Func<UInt128, long> factory);

    long GetOrAdd<TKey>(in TKey key, Func<UInt128, long> factory)
        where TKey : unmanaged;

    long GetOrAdd(in NerveCacheKey key, long value);

    long GetOrAdd<TKey>(in TKey key, long value)
        where TKey : unmanaged;
}
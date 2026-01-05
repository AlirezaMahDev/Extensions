using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerveCacheSection
{
    bool TryGet(in NerveCacheKey key, [NotNullWhen(true)] out DataOffset? value);

    bool TryGet<TKey>(in TKey key, [NotNullWhen(true)] out DataOffset? value)
        where TKey : unmanaged;

    DataOffset? GetOrNull(in NerveCacheKey key);

    DataOffset? GetOrNull<TKey>(in TKey key)
        where TKey : unmanaged;

    DataOffset Set(in NerveCacheKey key, DataOffset value);

    DataOffset Set<TKey>(in TKey key, DataOffset value)
        where TKey : unmanaged;

    DataOffset GetOrAdd(in NerveCacheKey key, Func<UInt128, DataOffset> factory);

    DataOffset GetOrAdd<TKey>(in TKey key, Func<UInt128, DataOffset> factory)
        where TKey : unmanaged;

    DataOffset GetOrAdd(in NerveCacheKey key, DataOffset value);

    DataOffset GetOrAdd<TKey>(in TKey key, DataOffset value)
        where TKey : unmanaged;
}
namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface INerveCacheSection
{
    bool TryGet(ref readonly NerveCacheKey key, [NotNullWhen(true)] out DataOffset? value);

    bool TryGet<TKey>(ref readonly TKey key, [NotNullWhen(true)] out DataOffset? value)
        where TKey : unmanaged;

    DataOffset? GetOrNull(ref readonly NerveCacheKey key);

    DataOffset? GetOrNull<TKey>(ref readonly TKey key)
        where TKey : unmanaged;

    void Set(ref readonly NerveCacheKey key, ref readonly DataOffset value);

    void Set<TKey>(ref readonly TKey key, ref readonly DataOffset value)
        where TKey : unmanaged;

    bool TrySet(ref readonly NerveCacheKey key, ref readonly DataOffset value);

    bool TrySet<TKey>(ref readonly TKey key, ref readonly DataOffset value)
        where TKey : unmanaged;

    DataOffset GetOrAdd(ref readonly NerveCacheKey key, ref readonly DataOffset value);

    DataOffset GetOrAdd<TKey>(ref readonly TKey key, ref readonly DataOffset value)
        where TKey : unmanaged;
}
namespace AlirezaMahDev.Extensions.Brain;

internal sealed class NerveCacheSection : INerveCacheSection, IDisposable
{
    private readonly NerveCacheSectionDictionary _cache = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static UInt128 GenerateHash<TKey>(ref readonly TKey key)
        where TKey : unmanaged
    {
        return NerveCacheKey.Create(in key).Hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGet<TKey>(ref readonly TKey key, [NotNullWhen(true)] out DataOffset? value)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        return TryGetCore(ref hash, out value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGet(ref readonly NerveCacheKey key, [NotNullWhen(true)] out DataOffset? value)
    {
        return TryGetCore(in key.Hash, out value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool TryGetCore(ref readonly UInt128 key, [NotNullWhen(true)] out DataOffset? value)
    {
        return (value = GetOrDefaultCore(in key)) is not null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset? GetOrNull<TKey>(ref readonly TKey key)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        return GetOrDefaultCore(ref hash) is { IsDefault: false } value ? value : null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset? GetOrNull(ref readonly NerveCacheKey key)
    {
        return GetOrDefaultCore(in key.Hash);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset? GetOrDefaultCore(ref readonly UInt128 key)
    {
        return _cache.TryGetValue(in key, out var result) ? result : null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Set<TKey>(ref readonly TKey key, ref readonly DataOffset value)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        SetCore(ref hash, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Set(ref readonly NerveCacheKey key, ref readonly DataOffset value)
    {
        SetCore(in key.Hash, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void SetCore(ref readonly UInt128 key, ref readonly DataOffset value)
    {
        _cache.Set(in key, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd<TKey>(ref readonly TKey key, Func<UInt128, DataOffset> factory)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        return GetOrAddCore(ref hash, factory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd(ref readonly NerveCacheKey key, Func<UInt128, DataOffset> factory)
    {
        return GetOrAddCore(in key.Hash, factory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd<TKey>(ref readonly TKey key, ref readonly DataOffset value)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        return GetOrAddCore(ref hash, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd(ref readonly NerveCacheKey key, ref readonly DataOffset value)
    {
        return GetOrAddCore(in key.Hash, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset GetOrAddCore(ref readonly UInt128 key, Func<UInt128, DataOffset> factory)
    {
        return _cache.GetOrAdd(in key, factory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset GetOrAddCore(ref readonly UInt128 key, ref readonly DataOffset value)
    {
        return _cache.GetOrAdd(in key, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TrySet(ref readonly NerveCacheKey key, ref readonly DataOffset value)
    {
        return TrySetCore(in key.Hash, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TrySet<TKey>(ref readonly TKey key, ref readonly DataOffset value)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        return TrySetCore(ref hash, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool TrySetCore(ref readonly UInt128 key, ref readonly DataOffset value)
    {
        return _cache.TrySet(in key, in value);
    }

    public void Dispose()
    {
        _cache.Dispose();
    }
}
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Brain;

public sealed class NerveCacheSectionDictionary
{
    private readonly Dictionary<UInt128, DataOffset> _cache = new(capacity: 1 << 16);
    private readonly ReaderWriterLockSlim _lockSlim = new();

    public DataOffset this[UInt128 key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set
        {
            _lockSlim.EnterWriteLock();
            _cache[key] = value;
            _lockSlim.ExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGetValue(UInt128 key, [NotNullWhen(true)] out DataOffset value)
    {
        _lockSlim.EnterReadLock();
        try
        {
            return _cache.TryGetValue(key, out value);
        }
        finally
        {
            _lockSlim.ExitReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd(UInt128 key, DataOffset value)
    {
        _lockSlim.EnterUpgradeableReadLock();
        try
        {
            if (TryGetValue(key, out var result))
            {
                return result;
            }

            result = value;
            this[key] = result;
            return result;

        }
        finally
        {
            _lockSlim.ExitUpgradeableReadLock();
        }

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd(UInt128 key, Func<UInt128, DataOffset> factory)
    {
        _lockSlim.EnterUpgradeableReadLock();
        try
        {
            if (TryGetValue(key, out var result))
            {
                return result;
            }

            result = factory(key);
            this[key] = result;
            return result;

        }
        finally
        {
            _lockSlim.ExitUpgradeableReadLock();
        }
    }
}

internal class NerveCacheSection : INerveCacheSection
{
    private readonly NerveCacheSectionDictionary _cache = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static UInt128 GenerateHash<TKey>(in TKey key)
        where TKey : unmanaged
    {
        return NerveCacheKey.Create(in key).Hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGet<TKey>(in TKey key, [NotNullWhen(true)] out DataOffset? value)
        where TKey : unmanaged
    {
        return TryGetCore(GenerateHash(in key), out value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGet(in NerveCacheKey key, [NotNullWhen(true)] out DataOffset? value)
    {
        return TryGetCore(key.Hash, out value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool TryGetCore(in UInt128 key, [NotNullWhen(true)] out DataOffset? value)
    {
        return (value = GetOrDefaultCore(key)) is not null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset? GetOrNull<TKey>(in TKey key)
        where TKey : unmanaged
    {
        return GetOrDefaultCore(GenerateHash(in key)) is { IsDefault: false } value ? value : null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset? GetOrNull(in NerveCacheKey key)
    {
        return GetOrDefaultCore(key.Hash);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset? GetOrDefaultCore(in UInt128 key)
    {
        return _cache.TryGetValue(key, out var result) ? result : null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset Set<TKey>(in TKey key, DataOffset value)
        where TKey : unmanaged
    {
        return SetCore(GenerateHash(in key), value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset Set(in NerveCacheKey key, DataOffset value)
    {
        return SetCore(key.Hash, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset SetCore(in UInt128 key, DataOffset value)
    {
        return _cache[key] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd<TKey>(in TKey key, Func<UInt128, DataOffset> factory)
        where TKey : unmanaged
    {
        return GetOrAddCore(GenerateHash(in key), factory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd(in NerveCacheKey key, Func<UInt128, DataOffset> factory)
    {
        return GetOrAddCore(key.Hash, factory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd<TKey>(in TKey key, DataOffset value)
        where TKey : unmanaged
    {
        return GetOrAddCore(GenerateHash(in key), value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd(in NerveCacheKey key, DataOffset value)
    {
        return GetOrAddCore(key.Hash, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset GetOrAddCore(in UInt128 key, Func<UInt128, DataOffset> factory)
    {
        return _cache.GetOrAdd(key, factory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset GetOrAddCore(in UInt128 key, DataOffset value)
    {
        return _cache.GetOrAdd(key, value);
    }
}
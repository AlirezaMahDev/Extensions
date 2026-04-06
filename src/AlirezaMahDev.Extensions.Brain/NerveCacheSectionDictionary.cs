namespace AlirezaMahDev.Extensions.Brain;

public sealed class NerveCacheSectionDictionary : IDisposable
{
    private const int ShardingCount = byte.MaxValue + 1;

    private readonly (Dictionary<UInt128, DataOffset> cache, ReaderWriterLockSlim lockSlim)[] _sharding =
        new (Dictionary<UInt128, DataOffset> cache, ReaderWriterLockSlim lockSlim)[ShardingCount];

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public NerveCacheSectionDictionary()
    {
        for (var i = 0; i < _sharding.Length; i++)
        {
            _sharding[i] = new(new(1 << 16), new());
        }
    }

    public DataOffset this[ref readonly UInt128 key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set => Set(in key, in value);
    }

    [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private ref (Dictionary<UInt128, DataOffset> cache, ReaderWriterLockSlim lockSlim) GetSharding(ref readonly UInt128 key)
    {
        return ref _sharding[(((ulong)key ^ (ulong)(key >> 64)) * 0x9E3779B97F4A7C15UL) >> 56];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Set(ref readonly UInt128 key, in DataOffset value)
    {
        var (cache, lockSlim) = GetSharding(in key);
        SetCore(in key, in value, cache, lockSlim);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void SetCore(ref readonly UInt128 key,
        in DataOffset value,
        Dictionary<UInt128, DataOffset> cache,
        ReaderWriterLockSlim lockSlim)
    {
        lockSlim.EnterWriteLock();
        cache[key] = value;
        lockSlim.ExitWriteLock();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGetValue(ref readonly UInt128 key, out DataOffset value)
    {
        var (cache, lockSlim) = GetSharding(in key);
        return TryGetValueCore(in key, out value, cache, lockSlim);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool TryGetValueCore(ref readonly UInt128 key,
        out DataOffset value,
        Dictionary<UInt128, DataOffset> cache,
        ReaderWriterLockSlim lockSlim)
    {
        lockSlim.EnterReadLock();
        try
        {
            return TryGetValueCoreRuntime(in key, out value, cache);
        }
        finally
        {
            lockSlim.ExitReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool TryGetValueCoreRuntime(ref readonly UInt128 key,
        out DataOffset value,
        Dictionary<UInt128, DataOffset> cache)
    {
        return cache.TryGetValue(key, out value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd(ref readonly UInt128 key, in DataOffset value)
    {
        var (cache, lockSlim) = GetSharding(in key);

        if (TryGetValueCore(in key, out var result, cache, lockSlim))
        {
            return result;
        }

        lockSlim.EnterUpgradeableReadLock();
        try
        {
            if (TryGetValueCoreRuntime(in key, out result, cache))
            {
                return result;
            }

            result = value;
            SetCore(in key, in result, cache, lockSlim);
            return result;
        }
        finally
        {
            lockSlim.ExitUpgradeableReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd(ref readonly UInt128 key, Func<UInt128, DataOffset> factory)
    {
        var (cache, lockSlim) = GetSharding(in key);

        if (TryGetValueCore(in key, out var result, cache, lockSlim))
        {
            return result;
        }

        lockSlim.EnterUpgradeableReadLock();
        try
        {
            if (TryGetValueCoreRuntime(in key, out result, cache))
            {
                return result;
            }

            result = factory(key);
            SetCore(in key, in result, cache, lockSlim);
            return result;
        }
        finally
        {
            lockSlim.ExitUpgradeableReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TrySet(ref readonly UInt128 key, in DataOffset value)
    {
        var (cache, lockSlim) = GetSharding(in key);
        if (TryGetValueCore(in key, out var result, cache, lockSlim))
        {
            return false;
        }

        lockSlim.EnterUpgradeableReadLock();
        try
        {
            if (TryGetValueCoreRuntime(in key, out result, cache))
            {
                return false;
            }

            result = value;
            SetCore(in key, in result, cache, lockSlim);
            return true;
        }
        finally
        {
            lockSlim.ExitUpgradeableReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TrySet(ref readonly UInt128 key, Func<UInt128, DataOffset> factory)
    {
        var (cache, lockSlim) = GetSharding(in key);
        if (TryGetValueCore(in key, out var result, cache, lockSlim))
        {
            return false;
        }

        lockSlim.EnterUpgradeableReadLock();
        try
        {
            if (TryGetValueCoreRuntime(in key, out result, cache))
            {
                return false;
            }

            result = factory(key);
            SetCore(in key, in result, cache, lockSlim);
            return true;
        }
        finally
        {
            lockSlim.ExitUpgradeableReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        foreach (var sharding in _sharding)
        {
            sharding.lockSlim.Dispose();
        }
    }
}
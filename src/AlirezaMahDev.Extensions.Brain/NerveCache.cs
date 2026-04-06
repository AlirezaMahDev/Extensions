using FASTER.core;

namespace AlirezaMahDev.Extensions.Brain;

using Session = ClientSession<
    NerveCacheIdKey, DataOffset, DataOffset, DataOffset,
    Empty, SimpleFunctions<NerveCacheIdKey, DataOffset, Empty>>;

internal sealed class NerveCache : INerveCache, IDisposable
{
    private const int IndexBits = 24;
    private const int LogBits = 29;
    private const int PageBits = 22;
    private const int SegmentBits = 30;
    private const int ReadCacheBits = 27;

    private readonly ConcurrentDictionary<string, NerveCacheSection> _cache = [];
    private readonly IDevice _logDevice;

    private FasterKV<NerveCacheIdKey, DataOffset> Kv
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    public ThreadLocal<Session> Session
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal NerveCache(string path)
    {
        _logDevice = Devices.CreateLogDevice(Path.Combine(path, "cache", "device.log"));
        Kv = new(size: 1L << IndexBits,
            logSettings: new()
            {
                LogDevice = _logDevice,
                ObjectLogDevice = new NullDevice(),
                MemorySizeBits = LogBits,
                PageSizeBits = PageBits,
                SegmentSizeBits = SegmentBits,
                ReadCacheSettings = new()
                {
                    MemorySizeBits = ReadCacheBits,
                    PageSizeBits = PageBits,
                }
            },
            comparer: new NerveCacheIdKeyComparer());
        Session = new(
            () => Kv.For(new SimpleFunctions<NerveCacheIdKey, DataOffset, Empty>())
                .NewSession<SimpleFunctions<NerveCacheIdKey, DataOffset, Empty>>(),
            trackAllValues: true
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Flush()
    {
        foreach (var session in Session.Values)
        {
            session.CompletePending(true);
        }

        Kv.Log.FlushAndEvict(wait: true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (Session.IsValueCreated)
        {
            foreach (var s in Session.Values)
            {
                s.Dispose();
            }
        }

        Kv.Dispose();
        _logDevice.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public INerveCacheSection GetOrAdd(string key)
    {
        return _cache.GetOrAdd(key, static (key, arg) => new(key, arg), this);
    }
}
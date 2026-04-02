using System.IO.Hashing;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

using FASTER.core;

namespace AlirezaMahDev.Extensions.Brain;

using Session = ClientSession<
    NerveCacheIdKey, DataOffset, DataOffset, DataOffset,
    Empty, SimpleFunctions<NerveCacheIdKey, DataOffset, Empty>>;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct NerveCacheIdKey(UInt128 id, UInt128 key)
{
    public readonly UInt128 Id = id;
    public readonly UInt128 Key = key;
}

public struct NerveCacheIdKeyComparer : IFasterEqualityComparer<NerveCacheIdKey>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ref NerveCacheIdKey k1, ref NerveCacheIdKey k2)
        => Vector256.LoadUnsafe(ref Unsafe.As<NerveCacheIdKey, byte>(ref k1)) ==
           Vector256.LoadUnsafe(ref Unsafe.As<NerveCacheIdKey, byte>(ref k2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetHashCode64(ref NerveCacheIdKey key)
    {
        return (long)XxHash3.HashToUInt64(MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref key, 1)));
    }
}

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
    public async ValueTask CheckpointAsync(CancellationToken ct = default)
    {
        var (ok, _) = await Kv.TakeHybridLogCheckpointAsync(
            CheckpointType.FoldOver,
            cancellationToken: ct);
        if (!ok) throw new InvalidOperationException("Checkpoint failed.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public async ValueTask RecoverAsync(CancellationToken ct = default)
        => await Kv.RecoverAsync(cancellationToken: ct);

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
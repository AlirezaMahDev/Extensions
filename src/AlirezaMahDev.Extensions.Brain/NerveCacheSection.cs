using System.Diagnostics;
using System.IO.Hashing;
using System.Text;

using FASTER.core;

namespace AlirezaMahDev.Extensions.Brain;

using Session = ClientSession<
    NerveCacheIdKey, DataOffset, DataOffset, DataOffset,
    Empty, SimpleFunctions<NerveCacheIdKey, DataOffset, Empty>>;

internal sealed class NerveCacheSection(string name, NerveCache cache) : INerveCacheSection
{
    private readonly UInt128 _id =
        XxHash128.HashToUInt128(Encoding.UTF8.GetBytes(name));

    // ── helpers ──────────────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private NerveCacheIdKey MakeKey(in UInt128 keyHash)
        => new(_id, keyHash);

    private Session Session
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return cache.Session.Value!;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static UInt128 GenerateHash<TKey>(ref readonly TKey key)
        where TKey : unmanaged
        => NerveCacheKey.Create(in key).Hash;

    // ── TryGet ───────────────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGet<TKey>(ref readonly TKey key, [NotNullWhen(true)] out DataOffset? value)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        return TryGetCore(in hash, out value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGet(ref readonly NerveCacheKey key, [NotNullWhen(true)] out DataOffset? value)
        => TryGetCore(in key.Hash, out value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool TryGetCore(ref readonly UInt128 keyHash, [NotNullWhen(true)] out DataOffset? value)
    {
        value = GetCoreRuntime(in keyHash);
        return value is not null;
    }

    // ── GetOrNull ────────────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset? GetOrNull<TKey>(ref readonly TKey key)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        return GetCoreRuntime(in hash);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset? GetOrNull(ref readonly NerveCacheKey key)
        => GetCoreRuntime(in key.Hash);

    // ── Set ──────────────────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Set<TKey>(ref readonly TKey key, ref readonly DataOffset value)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        SetCoreRuntime(in hash, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Set(ref readonly NerveCacheKey key, ref readonly DataOffset value)
        => SetCoreRuntime(in key.Hash, in value);

    // ── GetOrAdd ─────────────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd<TKey>(ref readonly TKey key, Func<UInt128, DataOffset> factory)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        return GetOrAddCore(in hash, factory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd(ref readonly NerveCacheKey key, Func<UInt128, DataOffset> factory)
        => GetOrAddCore(in key.Hash, factory);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd<TKey>(ref readonly TKey key, ref readonly DataOffset value)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        return GetOrAddCore(in hash, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset GetOrAdd(ref readonly NerveCacheKey key, ref readonly DataOffset value)
        => GetOrAddCore(in key.Hash, in value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset GetOrAddCore(ref readonly UInt128 keyHash, Func<UInt128, DataOffset> factory)
    {
        var existing = GetCoreRuntime(in keyHash);
        if (existing.HasValue)
            return existing.Value;

        var newVal = factory(keyHash);
        return GetOrAddCore(in keyHash, in newVal);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset GetOrAddCore(ref readonly UInt128 keyHash, ref readonly DataOffset value)
    {
        return GetOrAddCoreRuntime(in keyHash, in value);
    }

    // ── Core Operations ──────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset? GetCoreRuntime(ref readonly UInt128 keyHash)
    {
        var copyKey = MakeKey(in keyHash);
        DataOffset result = default;
        var status = Session.Read(ref copyKey, ref result);

        if (status.Found)
        {
            return result;
        }

        if (status.NotFound)
        {
            return null;
        }

        if (status.IsPending)
        {
            Session.CompletePendingWithOutputs(out var outputs, wait: true);
            using (outputs)
            {
                while (outputs.Next())
                {
                    if (outputs.Current.Output.IsDefault)
                        Debugger.Break();

                    status = outputs.Current.Status;
                    result = outputs.Current.Output;
                    if (status.Found)
                    {
                        return result;
                    }

                    if (status.NotFound)
                    {
                        return null;
                    }

                    throw new("Unexpected status");
                }
            }
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void SetCoreRuntime(ref readonly UInt128 keyHash, ref readonly DataOffset value)
    {
        var copyKey = MakeKey(in keyHash);
        var copyValue = value;
        if (copyValue.IsDefault)
            Debugger.Break();
        var status = Session.Upsert(ref copyKey, ref copyValue);

        if (status.IsPending)
        {
            Session.CompletePending(wait: false);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset GetOrAddCoreRuntime(ref readonly UInt128 keyHash, ref readonly DataOffset value)
    {
        var copyKey = MakeKey(in keyHash);
        var copyValue = value;
        if (copyValue.IsDefault)
            Debugger.Break();
        DataOffset result = default;
        var status = Session.RMW(ref copyKey, ref copyValue, ref result);

        if (status.IsPending)
        {
            Session.CompletePendingWithOutputs(out var completedOutputs, wait: true);
            using (completedOutputs)
            {
                while (completedOutputs.Next())
                {
                    if (completedOutputs.Current.Output.IsDefault)
                        Debugger.Break();

                    result = completedOutputs.Current.Output;
                    break;
                }
            }
        }

        if (result.IsDefault)
            Debugger.Break();
        return result;
    }
}
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
        value = ReadCore(in keyHash);
        return value is not null;
    }

    // ── GetOrNull ────────────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset? GetOrNull<TKey>(ref readonly TKey key)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        return ReadCore(in hash);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DataOffset? GetOrNull(ref readonly NerveCacheKey key)
        => ReadCore(in key.Hash);

    // ── Set ──────────────────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Set<TKey>(ref readonly TKey key, ref readonly DataOffset value)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        UpsertCore(in hash, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Set(ref readonly NerveCacheKey key, ref readonly DataOffset value)
        => UpsertCore(in key.Hash, in value);

    // ── TrySet (فقط اگه وجود نداره set کن) ──────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TrySet<TKey>(ref readonly TKey key, ref readonly DataOffset value)
        where TKey : unmanaged
    {
        var hash = GenerateHash(in key);
        return TrySetCore(in hash, in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TrySet(ref readonly NerveCacheKey key, ref readonly DataOffset value)
        => TrySetCore(in key.Hash, in value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool TrySetCore(ref readonly UInt128 keyHash, ref readonly DataOffset value)
    {
        // اگه موجوده → false برگردون
        if (ReadCore(in keyHash) is not null) return false;
        UpsertCore(in keyHash, in value);
        return true;
        // ⚠️ non-atomic: اگه atomicity لازم داری → RMW بزن (ببین پایین)
    }

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
        var existing = ReadCore(in keyHash);
        if (existing is not null) return existing.Value;

        var newVal = factory(keyHash);
        UpsertCore(in keyHash, in newVal);
        return newVal;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset GetOrAddCore(ref readonly UInt128 keyHash, ref readonly DataOffset value)
    {
        var existing = ReadCore(in keyHash);
        if (existing is not null) return existing.Value;

        UpsertCore(in keyHash, in value);
        return value;
    }

    // ── Core Operations ──────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private DataOffset? ReadCore(ref readonly UInt128 keyHash)
    {
        var ck = MakeKey(in keyHash);
        DataOffset output = default;
        var status = Session.Read(ref ck, ref output);

        if (status.IsCompletedSuccessfully && status.Found)
            return output;

        if (status.IsPending)
        {
            Session.CompletePendingWithOutputs(out var outputs, wait: true);
            using (outputs)
            {
                while (outputs.Next())
                    return outputs.Current.Output;
            }
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void UpsertCore(ref readonly UInt128 keyHash, ref readonly DataOffset value)
    {
        var ck = MakeKey(in keyHash);
        var val = value; // ref readonly → local copy برای ref
        var status = Session.Upsert(ref ck, ref val);

        if (status.IsPending)
            Session.CompletePending(wait: false);
    }
}
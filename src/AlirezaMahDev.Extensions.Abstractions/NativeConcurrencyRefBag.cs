namespace AlirezaMahDev.Extensions.Abstractions;

[CollectionBuilder(typeof(NativeConcurrencyRefBagCollectionBuilder), nameof(NativeConcurrencyRefBagCollectionBuilder.Create))]
public struct NativeConcurrencyRefBag<T> : IConcurrencyRefBag<NativeConcurrencyRefBag<T>, T>, IDisposable
    where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefBag<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefBag<T> Create(params ReadOnlySpan<T> values)
    {
        if (values.Length == 0)
        {
            return NativeConcurrencyRefBag<T>.Create();
        }
        var list = NativeConcurrencyRefBag<T>.Create();
        Span<ConcurrencyIndex> span = stackalloc ConcurrencyIndex[values.Length];
        list.TryAdd(span, values);
        return list;
    }

    private NativeRefList<NativeLockRefList<T>> _sharding;

    private volatile int _length;
    private static readonly int ShardingCount = Environment.ProcessorCount;
    private static readonly int SharingGap = 1 << 16;
    private static readonly int SharingLength = ShardingCount * SharingGap;
    private static int SharingId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return Environment.CurrentManagedThreadId % ShardingCount * SharingGap;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public NativeConcurrencyRefBag(int capacity, bool init)
    {
        _sharding = NativeRefList<NativeLockRefList<T>>.Create(SharingLength, true);
        for (var index = 0; index < _sharding.Length; index++)
        {
            _sharding[index] = NativeLockRefList<T>.Create(capacity, init);
        }
    }

    public LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _sharding[index.ShardingIndex][index.ShardingItemIndex];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGet(in ConcurrencyIndex index, out LockRefItem<T> result)
    {
        return _sharding[index.ShardingIndex].TryGet(index.ShardingItemIndex, out result)!.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryRemove(in ConcurrencyIndex index, out T result)
    {
        if (_sharding[index.ShardingIndex].TryRemove(index.ShardingItemIndex, out result)!.Value)
        {
            Interlocked.Decrement(ref _length);
            return true;
        }
        return false;
    }

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly int GetShardingLength()
    {
        return _sharding.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingItemsLength(int shardingIndex)
    {
        return _sharding[shardingIndex].Length;
    }

    private ref NativeLockRefList<T> GetFreeSharding(out int shardingIndex, ref int sharingGapIndex)
    {
        SpinWait spin = default;
        var sharingId = SharingId;
        while (true)
        {
            shardingIndex = (sharingId + sharingGapIndex) % SharingLength;
            ref var shading = ref _sharding[shardingIndex];
            if (shading.IsFree)
            {
                return ref shading;
            }
            sharingGapIndex++;
            if (sharingGapIndex >= SharingLength)
            {
                sharingGapIndex = 0;
                spin.SpinOnce();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConcurrencyIndex TryAdd(in T value)
    {
        var gapIndex = 0;
        while (true)
        {
            ref var sharing = ref GetFreeSharding(out var sharingId, ref gapIndex);
            var shardingIndex = sharing.TryAdd(in value, 0);
            if (shardingIndex is null)
            {
                gapIndex++;
                continue;
            }

            Interlocked.Increment(ref _length);
            return new ConcurrencyIndex(sharingId, shardingIndex.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryAdd(Span<ConcurrencyIndex> indices, params ReadOnlySpan<T> values)
    {
        var gapIndex = 0;
        while (true)
        {
            ref var sharing = ref GetFreeSharding(out var sharingId, ref gapIndex);
            var shardingIndex = sharing.TryAdd(values, 0);
            if (shardingIndex is null)
            {
                gapIndex++;
                continue;
            }

            if (shardingIndex == -1)
            {
                return false;
            }

            Interlocked.Add(ref _length, indices.Length);
            for (var i = 0; i < indices.Length; i++)
            {
                indices[i] = new ConcurrencyIndex(sharingId, shardingIndex.Value + i);
            }
            return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Insert(int index, in T value)
    {
        var gapIndex = 0;
        while (true)
        {
            ref var sharing = ref GetFreeSharding(out var sharingId, ref gapIndex);
            var tryInsert = sharing.TryInsert(index, in value, 0);
            if (tryInsert is null)
            {
                gapIndex++;
                continue;
            }

            if (!tryInsert.Value)
            {
                return false;
            }

            Interlocked.Increment(ref _length);
            return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Insert(int index, params ReadOnlySpan<T> values)
    {
        var gapIndex = 0;
        while (true)
        {
            ref var sharing = ref GetFreeSharding(out var sharingId, ref gapIndex);
            var tryInsert = sharing.TryInsert(index, values, 0);
            if (tryInsert is null)
            {
                gapIndex++;
                continue;
            }
            if (!tryInsert.Value)
            {
                return false;
            }

            Interlocked.Increment(ref _length);
            return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGet(out LockRefItem<T> result)
    {
        int gapIndex = 0;
        while (_length > 0)
        {
            ref var sharing = ref GetFreeSharding(out var sharingId, ref gapIndex);
            var tryGet = sharing.TryGet(sharing.Length - 1, out result, 0);
            if (tryGet == null)
            {
                gapIndex++;
                continue;
            }

            return tryGet.Value;
        }

        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryRemove(out T result)
    {
        int gapIndex = 0;
        while (_length > 0)
        {
            ref var sharing = ref GetFreeSharding(out var sharingId, ref gapIndex);
            var tryRemove = sharing.TryRemove(sharing.Length - 1, out result, 0);
            if (tryRemove is null)
            {
                gapIndex++;
                continue;
            }

            if (!tryRemove.Value)
            {
                return false;
            }

            Interlocked.Decrement(ref _length);
            return true;
        }

        result = default;
        return false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly ConcurrencyRefIndexableEnumerator<NativeConcurrencyRefBag<T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        for (var index = 0; index < _sharding.Length; index++)
        {
            _sharding[index].Clean();
        }
        Interlocked.Exchange(ref _length, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        for (var index = 0; index < _sharding.Length; index++)
        {
            _sharding[index].Dispose();
        }
        _sharding.Dispose();
    }
}
using System.Diagnostics.CodeAnalysis;
using System.IO.Hashing;
using System.Runtime.Intrinsics;

namespace AlirezaMahDev.Extensions.Abstractions;

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct ConcurrencyIndex(int shardingIndex, int shardingItemIndex) : IScopedRefReadOnlyEquatable<ConcurrencyIndex>, IScopedInEqualityOperators<ConcurrencyIndex, ConcurrencyIndex, bool>
{
    public static readonly ConcurrencyIndex Null = new(-1, -1);
    public readonly int ShardingIndex = shardingIndex;
    public readonly int ShardingItemIndex = shardingItemIndex;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ref ConcurrencyIndex FromRefLong(ref long value) =>
        ref Unsafe.As<long, ConcurrencyIndex>(ref value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly ref long ToRefLong()
    {
        return ref Unsafe.As<ConcurrencyIndex, long>(ref Unsafe.AsRef(in this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(scoped ref readonly ConcurrencyIndex other)
    {
        return ToRefLong() == other.ToRefLong();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(scoped in ConcurrencyIndex left, scoped in ConcurrencyIndex right)
    {
        return left.Equals(in right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(scoped in ConcurrencyIndex left, scoped in ConcurrencyIndex right)
    {
        return !left.Equals(in right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ConcurrencyIndex concurrencyIndex && Equals(in concurrencyIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return XxHash3.Combine(in ShardingIndex, in ShardingItemIndex);
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct ConcurrencyRefIndexableItem<TSelf, T>(TSelf self, ConcurrencyIndex index)
where TSelf : IConcurrencyRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    public ConcurrencyIndex Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = index;
    public LockRefItem<T> Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _self[Index];
        }
    }
}

public interface IConcurrencyRefIndexable<TSelf, T> : ILockRefEnumerable<TSelf, T, ConcurrencyRefIndexableEnumerator<TSelf, T>>, IRefLength
    where TSelf : IConcurrencyRefIndexable<TSelf, T>, allows ref struct
{
    int GetShardingLength();
    int GetShardingItemsLength(int shardingIndex);

    LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}

public interface IConcurrencyRefReadOnlyIndexable<TSelf, T> : ILockRefReadOnlyIndexable<TSelf, T>
    where TSelf : IConcurrencyRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    LockRefReadOnlyItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct ConcurrencyRefIndexableEnumerator<TSelf, T>(TSelf self) : ILockRefEnumerator<ConcurrencyRefIndexableEnumerator<TSelf, T>, T>
where TSelf : IConcurrencyRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _shardingIndex = 0;
    private int _shardingItemIndex = -1;
    private readonly ConcurrencyIndex Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return new(_shardingIndex, _shardingItemIndex);
        }
    }

    public readonly LockRefItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _self[Index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        while (true)
        {
            _shardingItemIndex++;
            if (_shardingItemIndex < _self.GetShardingItemsLength(_shardingIndex))
            {
                break;
            }

            _shardingIndex++;

            if (_shardingIndex < _self.GetShardingLength())
            {
                _shardingItemIndex = -1;
                continue;
            }

            return false;
        }

        return true;
    }
}


[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct ConcurrencyRefReadOnlyIndexableEnumerator<TSelf, T>(TSelf self) : ILockRefReadOnlyEnumerator<ConcurrencyRefReadOnlyIndexableEnumerator<TSelf, T>, T>
where TSelf : IConcurrencyRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _shardingIndex = 0;
    private int _shardingItemIndex = -1;
    private readonly ConcurrencyIndex Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return new(_shardingIndex, _shardingItemIndex);
        }
    }

    public readonly LockRefReadOnlyItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _self[Index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        while (true)
        {
            _shardingItemIndex++;
            if (_shardingItemIndex < _self.GetShardingItemsLength(_shardingIndex))
            {
                break;
            }

            _shardingIndex++;

            if (_shardingIndex < _self.GetShardingLength())
            {
                _shardingItemIndex = -1;
                continue;
            }

            return false;
        }

        return true;
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct ConcurrencyRefEnumeratorToConcurrencyRefReadOnlyEnumerator<TConcurrencyRefEnumerator, T>(TConcurrencyRefEnumerator refEnumerator)
: ILockRefReadOnlyEnumerator<ConcurrencyRefEnumeratorToConcurrencyRefReadOnlyEnumerator<TConcurrencyRefEnumerator, T>, T>
where TConcurrencyRefEnumerator : ILockRefEnumerator<TConcurrencyRefEnumerator, T>, allows ref struct
{
    private readonly TConcurrencyRefEnumerator _refEnumerator = refEnumerator;
    public LockRefReadOnlyItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _refEnumerator.Current;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext() => _refEnumerator.MoveNext();
}

public interface IConcurrencyRefBag<TSelf, T> : IConcurrencyRefIndexable<TSelf, T>
    where TSelf : IConcurrencyRefBag<TSelf, T>, allows ref struct
{
    ConcurrencyIndex TryAdd(in T value);
    bool TryGet(in ConcurrencyIndex index, [NotNullWhen(true)] out LockRefItem<T> result);
    bool TryGet([NotNullWhen(true)] out LockRefItem<T> result);
    bool TryAdd(Span<ConcurrencyIndex> indices, params ReadOnlySpan<T> values);
    bool TryRemove(in ConcurrencyIndex index, [NotNullWhen(true)] out T? result);
    bool TryRemove([NotNullWhen(true)] out T? result);
    void Clean();
}

public interface IConcurrencyRefDictionary<TSelf, TKey, TValue> : IRefLength
    where TSelf : IConcurrencyRefDictionary<TSelf, TKey, TValue>, allows ref struct
{
    bool TryGet(in TKey key, out TValue value);
    bool TryRemove(in TKey key);
    bool TryAdd(in TKey key, in TValue value);
    bool TryAdd(in TKey key, ScopedRefReadOnlyFunc<TKey, TValue> func);
    TValue GetOrAdd(in TKey key, in TValue value);
    TValue GetOrAdd(in TKey key, ScopedRefReadOnlyFunc<TKey, TValue> func);
    void Clean();
}


public interface IConcurrencyRefStack<TSelf, T> : IConcurrencyRefIndexable<TSelf, T>
    where TSelf : IConcurrencyRefStack<TSelf, T>, allows ref struct
{
    bool TryPop([NotNullWhen(true)] out T? result);
    bool TryPeek([NotNullWhen(true)] out ConcurrencyRefIndexableItem<TSelf, T> result);
    bool TryPush(in T value);
    void Clean();
}

public interface IConcurrencyRefQueue<TSelf, T> : IConcurrencyRefIndexable<TSelf, T>
    where TSelf : IConcurrencyRefQueue<TSelf, T>, allows ref struct
{
    bool TryPeek([NotNullWhen(true)] out ConcurrencyRefIndexableItem<TSelf, T> result);
    bool TryDequeue([NotNullWhen(true)] out T? result);
    bool TryEnqueue(in T value);
    void Clean();
}

public interface IConcurrencyRefPool<TSelf, T> : IConcurrencyRefIndexable<TSelf, T>
    where TSelf : IConcurrencyRefPool<TSelf, T>, allows ref struct
{
    ConcurrencyRefIndexableItem<TSelf, T> Rent();
    void Return(ConcurrencyIndex index);
    void Clean();
}

public static class ConcurrencyRefPoolExtensions
{
    extension<TSelf, T>(TSelf self)
        where TSelf : IConcurrencyRefPool<TSelf, T>, allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ConcurrencyIndex Rent(T value)
        {
            var item = self.Rent();
            using LockRefItem<T> itemLocker = item.Value;
            itemLocker.Value = value;
            return item.Index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Return(in ConcurrencyRefIndexableItem<TSelf, T> item)
        {
            self.Return(item.Index);
        }
    }
}

public struct NativeConcurrencyRefBag<T> : IConcurrencyRefBag<NativeConcurrencyRefBag<T>, T>, IDisposable
    where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefBag<T> Create(int capacity = 1, bool init = false) => new(capacity, init);


    private NativeRefList<NativeLockRefList<T>> _sharding;

    private volatile int _length;
    private static readonly int ShardingCount = Environment.ProcessorCount;
    private static readonly int SharingGap = 4;
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
    public bool TryGet(in ConcurrencyIndex index, [NotNullWhen(true)] out LockRefItem<T> result)
    {
        return _sharding[index.ShardingIndex].TryGet(index.ShardingItemIndex, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryRemove(in ConcurrencyIndex index, [NotNullWhen(true)] out T result)
    {
        if (_sharding[index.ShardingIndex].TryRemove(index.ShardingItemIndex, out result))
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
            if (shardingIndex == -1)
            {
                gapIndex++;
                continue;
            }
            Interlocked.Increment(ref _length);
            return new ConcurrencyIndex(sharingId, shardingIndex);
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
            if (shardingIndex == -1)
            {
                gapIndex++;
                continue;
            }
            Interlocked.Add(ref _length, indices.Length);
            for (var i = 0; i < indices.Length; i++)
            {
                indices[i] = new ConcurrencyIndex(sharingId, shardingIndex + i);
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
            if (!sharing.TryInsert(index, in value, 0))
            {
                gapIndex++;
                continue;
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
            if (!sharing.TryInsert(index, values, 0))
            {
                gapIndex++;
                continue;
            }
            Interlocked.Increment(ref _length);
            return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGet([NotNullWhen(true)] out LockRefItem<T> result)
    {
        int gapIndex = 0;
        while (_length > 0)
        {
            ref var sharing = ref GetFreeSharding(out var sharingId, ref gapIndex);
            if (!sharing.TryGet(sharing.Length - 1, out result, 0))
            {
                gapIndex++;
                continue;
            }

            return true;
        }

        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryRemove([NotNullWhen(true)] out T result)
    {
        int gapIndex = 0;
        while (_length > 0)
        {
            ref var sharing = ref GetFreeSharding(out var sharingId, ref gapIndex);
            if (!sharing.TryRemove(sharing.Length - 1, out result, 0))
            {
                gapIndex++;
                continue;
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

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public sealed class NativeConcurrencyRefStack<T>(int capacity, bool init) : IConcurrencyRefStack<NativeConcurrencyRefStack<T>, T>, IDisposable
where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefStack<T> Create(int capacity = 1, bool init = false) => new(capacity, init);


    private NativeLockRefStack<ConcurrencyIndex> _stack = NativeLockRefStack<ConcurrencyIndex>.Create(capacity, init);
    private NativeConcurrencyRefBag<T> _bag = NativeConcurrencyRefBag<T>.Create(capacity, init);

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {

            return _stack.Length;
        }
    }

    public LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {

            return _bag[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {

        _stack.Dispose();
        _bag.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPop([NotNullWhen(true)] out T result)
    {

        if (!_stack.TryPop(out var index))
        {
            result = default;
            return false;
        }

        return _bag.TryRemove(index, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPeek([NotNullWhen(true)] out ConcurrencyRefIndexableItem<NativeConcurrencyRefStack<T>, T> result)
    {

        if (!_stack.TryPeek(out var index))
        {
            result = default;
            return false;
        }

        result = new(this, index.Value.CopyValue);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPush(in T value)
    {

        return _stack.TryPush(_bag.TryAdd(in value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {

        _stack.Clean();
        _bag.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConcurrencyRefIndexableEnumerator<NativeConcurrencyRefStack<T>, T> GetEnumerator()
    {

        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingLength()
    {

        return _bag.GetShardingLength();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingItemsLength(int shardingIndex)
    {

        return _bag.GetShardingItemsLength(shardingIndex);
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public sealed class NativeConcurrencyRefQueue<T>(int capacity, bool init) : IConcurrencyRefQueue<NativeConcurrencyRefQueue<T>, T>, IDisposable
where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefQueue<T> Create(int capacity = 1, bool init = false) => new(capacity, init);


    private NativeLockRefQueue<ConcurrencyIndex> _queue = NativeLockRefQueue<ConcurrencyIndex>.Create(capacity, init);
    private NativeConcurrencyRefBag<T> _bag = NativeConcurrencyRefBag<T>.Create(capacity, init);

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {

            return _bag.Length;
        }
    }

    public LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {

            return _bag[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPeek([NotNullWhen(true)] out ConcurrencyRefIndexableItem<NativeConcurrencyRefQueue<T>, T> result)
    {

        if (!_queue.TryPeek(out var index))
        {
            result = default;
            return false;
        }

        result = new(this, index.Value.CopyValue);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryDequeue([NotNullWhen(true)] out T result)
    {

        if (!_queue.TryDequeue(out var index))
        {
            result = default;
            return false;
        }

        return _bag.TryRemove(index, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryEnqueue(in T value)
    {

        return _queue.TryEnqueue(_bag.TryAdd(in value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {

        _queue.Dispose();
        _bag.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {

        _queue.Clean();
        _bag.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConcurrencyRefIndexableEnumerator<NativeConcurrencyRefQueue<T>, T> GetEnumerator()
    {

        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingLength()
    {

        return _bag.GetShardingLength();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingItemsLength(int shardingIndex)
    {

        return _bag.GetShardingItemsLength(shardingIndex);
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public struct NativeConcurrencyRefPool<T>(int capacity, bool init) : IConcurrencyRefPool<NativeConcurrencyRefPool<T>, T>, IDisposable
where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefPool<T> Create(int capacity = 1, bool init = false) => new(capacity, init);


    private readonly NativeConcurrencyRefBag<ConcurrencyIndex> _free = new(capacity, init);
    private readonly NativeConcurrencyRefBag<T> _used = new(capacity, init);

    public readonly LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {

            return _used[index];
        }
    }

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {

            return _used.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly ConcurrencyRefIndexableItem<NativeConcurrencyRefPool<T>, T> Rent()
    {
        return _free.TryRemove(out var item)
            ? new(this, item)
            : new(this, _used.TryAdd(default(T)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly void Return(ConcurrencyIndex index)
    {
        _free.TryAdd(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly ConcurrencyRefIndexableEnumerator<NativeConcurrencyRefPool<T>, T> GetEnumerator()
    {

        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly int GetShardingLength()
    {

        return _used.GetShardingLength();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly int GetShardingItemsLength(int shardingIndex)
    {

        return _used.GetShardingItemsLength(shardingIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly void Clean()
    {

        _free.Clean();
        _used.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly void Dispose()
    {

        _free.Dispose();
        _used.Dispose();
    }
}
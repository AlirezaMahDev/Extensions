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

    private ref long Long
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref Unsafe.As<ConcurrencyIndex, long>(ref Unsafe.AsRef(in this));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(scoped ref readonly ConcurrencyIndex other)
    {
        return Long == other.Long;
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
    ConcurrencyIndex Add(in T value);
    bool TryGet(in ConcurrencyIndex index, [NotNullWhen(true)] out LockRefItem<T> result);
    bool Add(Span<ConcurrencyIndex> indices, params ReadOnlySpan<T> values);
    bool Remove(in ConcurrencyIndex index, [NotNullWhen(true)] out T? result);
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

    private ReaderWriterLocker _locker;
    private NativeRefList<NativeLockRefList<T>> _sharding;

    private volatile int _length;
    private static readonly int ShardingCount = Environment.ProcessorCount * 4;
    private static int SharingId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return Environment.CurrentManagedThreadId & ShardingCount;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public NativeConcurrencyRefBag(int capacity, bool init)
    {
        _locker = new();
        _sharding = NativeRefList<NativeLockRefList<T>>.Create(ShardingCount, true);
        for (var index = 0; index < _sharding.Length; index++)
        {
            _sharding[index] = NativeLockRefList<T>.Create(capacity, init);
        }
    }

    private ref NativeLockRefList<T> GetFreeSharding()
    {
        var index = SharingId;
        while (true)
        {
            ref var sharding = ref _sharding[index];
            if (sharding.IsFree)
            {
                return ref sharding;
            }
            index = (index + 1) & ShardingCount;
        }
    }

    public LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _sharding[index.ShardingIndex][index.ShardingItemIndex];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryGet(in ConcurrencyIndex index, [NotNullWhen(true)] out LockRefItem<T> result)
    {
        using var @lock = _locker.EnterReadLockScope();
        return _sharding[index.ShardingIndex].TryGet(index.ShardingItemIndex, out result);
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingLength()
    {
        using var @lock = _locker.EnterReadLockScope();
        return _sharding.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingItemsLength(int shardingIndex)
    {
        using var @lock = _locker.EnterReadLockScope();
        return _sharding[shardingIndex].Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConcurrencyIndex Add(in T value)
    {
        using var @lock = _locker.EnterReadLockScope();
        int sharingId = SharingId;
        var shardingIndex = _sharding[sharingId].Add(in value);
        if (shardingIndex == -1)
        {
            return new(-1, -1);
        }
        Interlocked.Increment(ref _length);
        return new ConcurrencyIndex(sharingId, shardingIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Add(Span<ConcurrencyIndex> indices, params ReadOnlySpan<T> values)
    {
        using var @lock = _locker.EnterReadLockScope();
        int sharingId = SharingId;
        var shardingIndex = _sharding[sharingId].Add(values);
        if (shardingIndex == -1)
        {
            return false;
        }
        Interlocked.Add(ref _length, indices.Length);
        Span<ConcurrencyIndex> concurrencyIndices = stackalloc ConcurrencyIndex[indices.Length];
        for (var i = 0; i < indices.Length; i++)
        {
            concurrencyIndices[i] = new ConcurrencyIndex(sharingId, shardingIndex + i);
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Insert(in int index, in T value)
    {
        using var @lock = _locker.EnterReadLockScope();
        if (_sharding[SharingId].Insert(in index, in value))
        {
            Interlocked.Increment(ref _length);
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Insert(in int index, params ReadOnlySpan<T> values)
    {
        using var @lock = _locker.EnterReadLockScope();
        if (_sharding[SharingId].Insert(in index, values))
        {
            Interlocked.Add(ref _length, values.Length);
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Remove(in ConcurrencyIndex index, [NotNullWhen(true)] out T result)
    {
        using var @lock = _locker.EnterReadLockScope();
        if (_sharding[index.ShardingIndex].Remove(in index.ShardingItemIndex, out result))
        {
            Interlocked.Decrement(ref _length);
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConcurrencyRefIndexableEnumerator<NativeConcurrencyRefBag<T>, T> GetEnumerator()
    {
        using var @lock = _locker.EnterReadLockScope();
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        using var @lock = _locker.EnterWriteLockScope();
        for (var index = 0; index < _sharding.Length; index++)
        {
            _sharding[index].Clean();
        }
        Interlocked.Exchange(ref _length, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        using var @lock = _locker.EnterWriteLockScope();
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

    private ReaderWriterLocker _locker = new();
    private NativeLockRefStack<ConcurrencyIndex> _stack = NativeLockRefStack<ConcurrencyIndex>.Create(capacity, init);
    private NativeConcurrencyRefBag<T> _bag = NativeConcurrencyRefBag<T>.Create(capacity, init);

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _stack.Length;
        }
    }

    public LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _bag[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _stack.Dispose();
        _bag.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPop([NotNullWhen(true)] out T result)
    {
        using var @lock = _locker.EnterWriteLockScope();
        if (!_stack.TryPop(out var index))
        {
            result = default;
            return false;
        }

        return _bag.Remove(index, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPeek([NotNullWhen(true)] out ConcurrencyRefIndexableItem<NativeConcurrencyRefStack<T>, T> result)
    {
        using var @lock = _locker.EnterReadLockScope();
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
        using var @lock = _locker.EnterWriteLockScope();
        return _stack.TryPush(_bag.Add(in value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _stack.Clean();
        _bag.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConcurrencyRefIndexableEnumerator<NativeConcurrencyRefStack<T>, T> GetEnumerator()
    {
        using var @lock = _locker.EnterReadLockScope();
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingLength()
    {
        using var @lock = _locker.EnterReadLockScope();
        return _bag.GetShardingLength();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingItemsLength(int shardingIndex)
    {
        using var @lock = _locker.EnterReadLockScope();
        return _bag.GetShardingItemsLength(shardingIndex);
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public sealed class NativeConcurrencyRefQueue<T>(int capacity, bool init) : IConcurrencyRefQueue<NativeConcurrencyRefQueue<T>, T>, IDisposable
where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefQueue<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    private ReaderWriterLocker _locker = new();
    private NativeLockRefQueue<ConcurrencyIndex> _queue = NativeLockRefQueue<ConcurrencyIndex>.Create(capacity, init);
    private NativeConcurrencyRefBag<T> _bag = NativeConcurrencyRefBag<T>.Create(capacity, init);

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _bag.Length;
        }
    }

    public LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _bag[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPeek([NotNullWhen(true)] out ConcurrencyRefIndexableItem<NativeConcurrencyRefQueue<T>, T> result)
    {
        using var @lock = _locker.EnterReadLockScope();
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
        using var @lock = _locker.EnterWriteLockScope();
        if (!_queue.TryDequeue(out var index))
        {
            result = default;
            return false;
        }

        return _bag.Remove(index, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryEnqueue(in T value)
    {
        using var @lock = _locker.EnterWriteLockScope();
        return _queue.TryEnqueue(_bag.Add(in value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _queue.Dispose();
        _bag.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _queue.Clean();
        _bag.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConcurrencyRefIndexableEnumerator<NativeConcurrencyRefQueue<T>, T> GetEnumerator()
    {
        using var @lock = _locker.EnterReadLockScope();
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingLength()
    {
        using var @lock = _locker.EnterReadLockScope();
        return _bag.GetShardingLength();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingItemsLength(int shardingIndex)
    {
        using var @lock = _locker.EnterReadLockScope();
        return _bag.GetShardingItemsLength(shardingIndex);
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public struct NativeConcurrencyRefPool<T>(int capacity, bool init) : IConcurrencyRefPool<NativeConcurrencyRefPool<T>, T>, IDisposable
where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefPool<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    private ReaderWriterLocker _locker = new();
    private readonly NativeConcurrencyRefStack<ConcurrencyIndex> _free = new(capacity, init);
    private readonly NativeConcurrencyRefBag<T> _used = new(capacity, init);

    public LockRefItem<T> this[ConcurrencyIndex index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _used[index];
        }
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _used.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly ConcurrencyRefIndexableItem<NativeConcurrencyRefPool<T>, T> Rent()
    {
        return _free.TryPop(out var item)
            ? new(this, item)
            : new(this, _used.Add(default(T)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly void Return(ConcurrencyIndex index)
    {
        _free.TryPush(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ConcurrencyRefIndexableEnumerator<NativeConcurrencyRefPool<T>, T> GetEnumerator()
    {
        using var @lock = _locker.EnterReadLockScope();
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingLength()
    {
        using var @lock = _locker.EnterReadLockScope();
        return _used.GetShardingLength();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int GetShardingItemsLength(int shardingIndex)
    {
        using var @lock = _locker.EnterReadLockScope();
        return _used.GetShardingItemsLength(shardingIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _free.Clean();
        _used.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _free.Dispose();
        _used.Dispose();
    }
}
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace AlirezaMahDev.Extensions.Abstractions;


public interface ILockRefEnumerator<TLockRefEnumerator, T>
    where TLockRefEnumerator : ILockRefEnumerator<TLockRefEnumerator, T>, allows ref struct
{
    LockRefItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool MoveNext();
}


public interface ILockRefReadOnlyEnumerator<TLockRefReadOnlyEnumerator, T>
    where TLockRefReadOnlyEnumerator : ILockRefReadOnlyEnumerator<TLockRefReadOnlyEnumerator, T>, allows ref struct
{
    LockRefReadOnlyItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool MoveNext();
}

public interface ILockRefEnumerableCore<TLockRefEnumeratorCore>
    where TLockRefEnumeratorCore : allows ref struct
{
    TLockRefEnumeratorCore GetEnumerator();
}

public interface ILockRefEnumerable<TSelf, T, TLockRefEnumerator> : ILockRefEnumerableCore<TLockRefEnumerator>
    where TSelf : ILockRefEnumerable<TSelf, T, TLockRefEnumerator>, allows ref struct
    where TLockRefEnumerator : ILockRefEnumerator<TLockRefEnumerator, T>, allows ref struct;


public interface ILockRefReadOnlyEnumerable<TSelf, T, TLockRefReadOnlyEnumerator> : ILockRefEnumerableCore<TLockRefReadOnlyEnumerator>
    where TSelf : ILockRefReadOnlyEnumerable<TSelf, T, TLockRefReadOnlyEnumerator>, allows ref struct
    where TLockRefReadOnlyEnumerator : ILockRefReadOnlyEnumerator<TLockRefReadOnlyEnumerator, T>, allows ref struct;

public interface ILockRefCountable<TSelf, T, TLockRefEnumerator> : ILockRefEnumerable<TSelf, T, TLockRefEnumerator>, IRefLength
    where TSelf : ILockRefCountable<TSelf, T, TLockRefEnumerator>, allows ref struct
    where TLockRefEnumerator : ILockRefEnumerator<TLockRefEnumerator, T>, allows ref struct;


public interface ILockRefReadOnlyCountable<TSelf, T, TLockRefReadOnlyEnumerator> : ILockRefReadOnlyEnumerable<TSelf, T, TLockRefReadOnlyEnumerator>, IRefLength
    where TSelf : ILockRefReadOnlyCountable<TSelf, T, TLockRefReadOnlyEnumerator>, allows ref struct
    where TLockRefReadOnlyEnumerator : ILockRefReadOnlyEnumerator<TLockRefReadOnlyEnumerator, T>, allows ref struct;


public interface ILockRefIndexable<TSelf, T> : ILockRefCountable<TSelf, T, LockRefIndexableEnumerator<TSelf, T>>, ILockerStatus
    where TSelf : ILockRefIndexable<TSelf, T>, allows ref struct
{
    LockRefItem<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}

public interface ILockRefReadOnlyIndexable<TSelf, T> : ILockRefReadOnlyCountable<TSelf, T, LockRefReadOnlyIndexableEnumerator<TSelf, T>>
    where TSelf : ILockRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    LockRefReadOnlyItem<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct LockRefIndexableEnumerator<TSelf, T>(TSelf self) : ILockRefEnumerator<LockRefIndexableEnumerator<TSelf, T>, T>
where TSelf : ILockRefIndexable<TSelf, T>, allows ref struct
{
    private int _index = -1;
    private readonly TSelf _self = self;

    public readonly LockRefItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _self[_index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        _index++;
        return _index < _self.Length;
    }
}

public ref struct LockRefReadOnlyIndexableEnumerator<TSelf, T>(TSelf self) : ILockRefReadOnlyEnumerator<LockRefReadOnlyIndexableEnumerator<TSelf, T>, T>
    where TSelf : ILockRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    private int _index = -1;
    private readonly TSelf _self = self;

    public readonly LockRefReadOnlyItem<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _self[_index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        _index++;
        return _index < _self.Length;
    }
}

public interface ILockRefBlock<TSelf, T> : ILockRefIndexable<TSelf, T>
    where TSelf : ILockRefBlock<TSelf, T>, allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    TSelf Slice(int start, int length);
}

public interface ILockRefReadOnlyBlock<TSelf, T> : ILockRefReadOnlyIndexable<TSelf, T>
    where TSelf : ILockRefReadOnlyBlock<TSelf, T>, allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    TSelf Slice(int start, int length);
}


[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct LockRefBlock<TSelf, T>(TSelf self) : ILockRefBlock<LockRefBlock<TSelf, T>, T>
where TSelf : ILockRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _start;
    private int _length;

    public readonly LockRefItem<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return index >= _length ? throw new IndexOutOfRangeException($"${index} >= {_length}") : _self[_start + index];
        }
    }
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _length;
        }
    }

    public readonly ReaderWriterLockerState LockerState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _self.LockerState;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly LockRefIndexableEnumerator<LockRefBlock<TSelf, T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public LockRefBlock<TSelf, T> Slice(int start, int length)
    {
        return start + length > _length
            ? throw new IndexOutOfRangeException($"{start + length} > {_length}")
            : (this with { _start = _start + start, _length = length });
    }
}


[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct LockRefReadOnlyBlock<TSelf, T>(TSelf self) : ILockRefReadOnlyBlock<LockRefReadOnlyBlock<TSelf, T>, T>
where TSelf : ILockRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _start;
    private int _length;

    public readonly LockRefReadOnlyItem<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return index >= _length ? throw new IndexOutOfRangeException($"{index} >= {_length}") : _self[_start + index];
        }
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
    public readonly LockRefReadOnlyIndexableEnumerator<LockRefReadOnlyBlock<TSelf, T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public LockRefReadOnlyBlock<TSelf, T> Slice(int start, int length)
    {
        return start + length > _length
            ? throw new IndexOutOfRangeException($"{start + length} > {_length}")
            : (this with { _start = _start + start, _length = length });
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct LockRefIndexableItem<TSelf, T>(TSelf self, int index)
where TSelf : ILockRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    public int Index
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

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct LockRefEnumeratorToLockRefReadOnlyEnumerator<TLockRefEnumerator, T>(TLockRefEnumerator refEnumerator)
: ILockRefReadOnlyEnumerator<LockRefEnumeratorToLockRefReadOnlyEnumerator<TLockRefEnumerator, T>, T>
where TLockRefEnumerator : ILockRefEnumerator<TLockRefEnumerator, T>, allows ref struct
{
    private readonly TLockRefEnumerator _refEnumerator = refEnumerator;
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

public interface ILockRefList<TSelf, T> : ILockRefIndexable<TSelf, T>
    where TSelf : ILockRefList<TSelf, T>, allows ref struct
{
    bool? TryGet(int index, out LockRefItem<T> result, int timeout = -1);
    int? TryAdd(in T value, int timeout = -1);
    int? TryAdd(ReadOnlySpan<T> values, int timeout = -1);
    bool? TryInsert(int index, in T value, int timeout = -1);
    bool? TryInsert(int index, ReadOnlySpan<T> values, int timeout = -1);
    bool? TryRemove(int index, [NotNullWhen(true)] out T? result, int timeout = -1);
    bool? TryRemove(int index, Span<T> result, int timeout = -1);
    void Clean();
}

public interface ILockRefDictionary<TSelf, TKey, TValue> : IRefLength
    where TSelf : ILockRefDictionary<TSelf, TKey, TValue>, allows ref struct
{
    bool TryGet(in TKey key, out TValue value);
    bool TryRemove(in TKey key);
    bool TryAdd(in TKey key, in TValue value);
    bool TryAdd(in TKey key, ScopedRefReadOnlyFunc<TKey, TValue> func);
    TValue GetOrAdd(in TKey key, in TValue value);
    TValue GetOrAdd(in TKey key, ScopedRefReadOnlyFunc<TKey, TValue> func);
    void Clean();
}


public interface ILockRefStack<TSelf, T> : ILockRefIndexable<TSelf, T>
    where TSelf : ILockRefStack<TSelf, T>, allows ref struct
{
    bool? TryPop([NotNullWhen(true)] out T? result, int timeout = -1);
    bool? TryPeek(out LockRefIndexableItem<TSelf, T> result, int timeout = -1);
    bool? TryPush(in T value, int timeout = -1);
    void Clean();
}

public interface ILockRefQueue<TSelf, T> : ILockRefIndexable<TSelf, T>
    where TSelf : ILockRefQueue<TSelf, T>, allows ref struct
{
    bool? TryPeek(out LockRefIndexableItem<TSelf, T> result, int timeout = -1);
    bool? TryDequeue([NotNullWhen(true)] out T? result, int timeout = -1);
    bool? TryEnqueue(in T value, int timeout = -1);
    void Clean();
}

public interface ILockRefPool<TSelf, T> : ILockRefIndexable<TSelf, T>
    where TSelf : ILockRefPool<TSelf, T>, allows ref struct
{
    bool? TryRent(out LockRefIndexableItem<TSelf, T> item, int timeout = -1);
    bool? TryReturn(int index, int timeout = -1);
    void Clean();
}
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct LockRefItem<T>(ref T value, ReaderWriterLockerReaderDispose dispose) : IDisposable
{
    private readonly ReaderWriterLockerReaderDispose _dispose = dispose;

    public ref T Value = ref value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly void Dispose()
    {
        _dispose.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LockRefReadOnlyItem<T>(LockRefItem<T> lockRefItem) =>
    new(ref lockRefItem.Value, lockRefItem._dispose);
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct LockRefReadOnlyItem<T>(ref readonly T value, ReaderWriterLockerReaderDispose dispose) : IDisposable
{
    private readonly ReaderWriterLockerReaderDispose _dispose = dispose;

    public ref readonly T Value = ref value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly void Dispose()
    {
        _dispose.Dispose();
    }
}


public static class NativeLockRefListCollectionBuilder
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeLockRefList<T> Create<T>(ReadOnlySpan<T> readOnlySpan)
        where T : unmanaged => NativeLockRefList<T>.Create(readOnlySpan);
}

[CollectionBuilder(typeof(NativeLockRefListCollectionBuilder), nameof(NativeLockRefListCollectionBuilder.Create))]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public struct NativeLockRefList<T>(int capacity, bool init) : ILockRefList<NativeLockRefList<T>, T>, IDisposable
where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeLockRefList<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeLockRefList<T> Create(params ReadOnlySpan<T> values)
    {
        var list = NativeLockRefList<T>.Create(values.Length, true);
        values.CopyTo(list._list.Span);
        return list;
    }

    private ReaderWriterLocker _locker = new();
    private NativeRefList<T> _list = new(capacity, init);

    public readonly ReaderWriterLockerState LockerState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _locker.LockerState;
        }
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _list.Length;
        }
    }

    public LockRefItem<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            var @lock = Unsafe.AsRef(in this)._locker.EnterReadLockScope();
            return new(ref _list[index], @lock);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryGet(int index, out LockRefItem<T> result, int timeout = -1)
    {
        if (!_locker.TryEnterReadLock(timeout))
        {
            result = default;
            return null;
        }
        try
        {
            if (index >= 0 && index < _list.Length)
            {
                result = this[index];
                return true;
            }

            result = default;
            return false;
        }
        finally
        {
            _locker.TryExitReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int? TryAdd(in T value, int timeout = -1)
    {
        if (!_locker.TryEnterWriteLock(timeout))
            return null;
        try
        {
            return _list.Add(in value);
        }
        finally
        {
            _locker.TryExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int? TryAdd(scoped ReadOnlySpan<T> values, int timeout = -1)
    {
        if (!_locker.TryEnterWriteLock(timeout))
            return null;
        try
        {
            return _list.Add(values);
        }
        finally
        {
            _locker.TryExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryInsert(int index, in T value, int timeout = -1)
    {
        if (!_locker.TryEnterWriteLock(timeout))
            return null;
        try
        {
            return _list.Insert(index, in value);
        }
        finally
        {
            _locker.TryExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryInsert(int index, scoped ReadOnlySpan<T> values, int timeout = -1)
    {
        if (!_locker.TryEnterWriteLock(timeout))
            return null;
        try
        {
            return _list.Insert(index, values);
        }
        finally
        {
            _locker.TryExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryRemove(int index, out T result, int timeout = -1)
    {
        if (!_locker.TryEnterWriteLock(timeout))
        {
            result = default;
            return null;
        }
        try
        {
            return _list.Remove(index, out result);
        }
        finally
        {
            _locker.TryExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryRemove(int index, Span<T> result, int timeout = -1)
    {
        if (!_locker.TryEnterWriteLock(timeout))
            return null;
        try
        {
            return _list.Remove(index, result);
        }
        finally
        {
            _locker.TryExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _list.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly LockRefIndexableEnumerator<NativeLockRefList<T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _list.Dispose();
    }
}


[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public struct NativeLockRefStack<T>(int capacity, bool init) : ILockRefStack<NativeLockRefStack<T>, T>, IDisposable
where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeLockRefStack<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    private ReaderWriterLocker _locker = new();
    private readonly NativeRefStack<T> _stack = new(capacity, init);

    public readonly ReaderWriterLockerState LockerState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _locker.LockerState;
        }
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _stack.Length;
        }
    }

    public readonly LockRefItem<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            var @lock = Unsafe.AsRef(in this)._locker.EnterReadLockScope();
            return new(ref _stack[index], @lock);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _stack.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryPop(out T result, int timeout = -1)
    {
        if (!_locker.TryEnterWriteLock(timeout))
        {
            result = default;
            return null;
        }
        try
        {
            return _stack.TryPop(out result);
        }
        finally
        {
            _locker.TryExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryPeek(out LockRefIndexableItem<NativeLockRefStack<T>, T> result, int timeout = -1)
    {
        if (!_locker.TryEnterReadLock(timeout))
        {
            result = default;
            return null;
        }
        try
        {
            if (_stack.TryPeek(out var refIndexableItem))
            {
                result = new(this, refIndexableItem.Index);
                return true;
            }
            result = default;
            return false;
        }
        finally
        {
            _locker.TryExitReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryPush(in T value, int timeout = -1)
    {
        if (!_locker.TryEnterWriteLock(timeout))
            return null;
        try
        {
            return _stack.TryPush(value);
        }
        finally
        {
            _locker.TryExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _stack.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public LockRefIndexableEnumerator<NativeLockRefStack<T>, T> GetEnumerator()
    {
        using var @lock = _locker.EnterReadLockScope();
        return new(this);
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public struct NativeLockRefQueue<T>(int capacity, bool init) : ILockRefQueue<NativeLockRefQueue<T>, T>, IDisposable
where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeLockRefQueue<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    private ReaderWriterLocker _locker = new();
    private readonly NativeRefQueue<T> _queue = new(capacity, init);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _queue.Dispose();
    }

    public readonly ReaderWriterLockerState LockerState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _locker.LockerState;
        }
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _queue.Length;
        }
    }

    public LockRefItem<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            var @lock = Unsafe.AsRef(in this)._locker.EnterReadLockScope();
            return new(ref _queue[index], @lock);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryPeek(out LockRefIndexableItem<NativeLockRefQueue<T>, T> result, int timeout = -1)
    {
        if (!_locker.TryEnterReadLock(timeout))
        {
            result = default;
            return null;
        }
        try
        {
            if (_queue.TryPeek(out var item))
            {
                result = new(this, item.Index);
                return true;
            }

            result = default;
            return false;
        }
        finally
        {
            _locker.TryExitReadLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryDequeue(out T result, int timeout = -1)
    {
        if (!_locker.TryEnterWriteLock(timeout))
        {
            result = default;
            return null;
        }
        try
        {
            return _queue.TryDequeue(out result);
        }
        finally
        {
            _locker.TryExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryEnqueue(in T value, int timeout = -1)
    {
        if (!_locker.TryEnterWriteLock(timeout))
            return null;
        try
        {
            return _queue.TryEnqueue(value);
        }
        finally
        {
            _locker.TryExitWriteLock();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        using var @lock = _locker.EnterWriteLockScope();
        _queue.Clean();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public LockRefIndexableEnumerator<NativeLockRefQueue<T>, T> GetEnumerator()
    {
        using var @lock = _locker.EnterReadLockScope();
        return new(this);
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public struct NativeLockRefPool<T>(int capacity, bool init) : ILockRefPool<NativeLockRefPool<T>, T>, IDisposable
where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeLockRefPool<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    private ReaderWriterLocker _locker = new();
    private readonly NativeLockRefStack<int> _free = new(capacity, init);
    private readonly NativeLockRefList<T> _used = new(capacity, init);

    public LockRefItem<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            using var @lock = _locker.EnterReadLockScope();
            return _used[index];
        }
    }

    public readonly ReaderWriterLockerState LockerState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _locker.LockerState;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryRent(out LockRefIndexableItem<NativeLockRefPool<T>, T> item, int timeout = -1)
    {
        using var @lock = _locker.EnterReadLockScope();
        var popResult = _free.TryPop(out var index, timeout);
        if (popResult is null)
        {
            item = default;
            return null;
        }

        if (popResult == true)
        {
            item = new(this, index);
            return true;
        }

        var newIndex = _used.TryAdd(default, timeout);
        if (newIndex is null)
        {
            item = default;
            return null;
        }

        if (newIndex == -1)
        {
            item = default;
            return false;
        }

        item = new(this, newIndex.Value);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool? TryReturn(int index, int timeout = -1)
    {
        using var @lock = _locker.EnterReadLockScope();
        return _free.TryPush(index, timeout);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public LockRefIndexableEnumerator<NativeLockRefPool<T>, T> GetEnumerator()
    {
        using var @lock = _locker.EnterReadLockScope();
        return new(this);
    }
}

public static class LockRefItemExtensions
{
    extension<T>(LockRefItem<T> self)
    {
        public T CopyValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                using var item = self;
                return item.Value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult GetCopyValue<TResult>(ScopedRefFunc<T, TResult> scopedRefFunc)
        {
            using var item = self;
            return scopedRefFunc(ref item.Value);
        }
    }
}
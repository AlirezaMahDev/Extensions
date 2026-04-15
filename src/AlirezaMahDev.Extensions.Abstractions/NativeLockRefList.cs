namespace AlirezaMahDev.Extensions.Abstractions;

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
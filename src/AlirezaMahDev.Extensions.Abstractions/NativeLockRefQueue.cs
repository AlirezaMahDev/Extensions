namespace AlirezaMahDev.Extensions.Abstractions;

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
namespace AlirezaMahDev.Extensions.Abstractions;

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
namespace AlirezaMahDev.Extensions.Abstractions;

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
using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Auto)]
[MustDisposeResource]
public readonly ref struct DataLockWriteDisposable<TValue> : IDisposable
    where TValue : unmanaged, IDataValue<TValue>
{
    private readonly CancellationToken _cancellationToken;
    private readonly bool _isChild;
    private readonly ref readonly DataLocation<TValue> _location;
    private readonly DataMapFilePartCacheAccess _cache;
    private readonly ref TValue _pointer;

    public DataLockWriteDisposable(ref readonly DataLocation<TValue> location,
        CancellationToken cancellationToken = default)
    {
        _cancellationToken = cancellationToken;
        _location = ref location;
        _cache = location.Access.GetCache(in _location.Offset, cancellationToken);
        try
        {
            _pointer = ref Unsafe.As<byte, TValue>(ref _cache.Cache.EnterAccessRefByte(_location.Offset.Offset));
            ref var @lock = ref _pointer.Lock;
            ref var ulongLock = ref Unsafe.As<DataLock, ulong>(ref @lock);
            SpinWait spinner = default;
            while (!cancellationToken.IsCancellationRequested)
            {
                var lastUlongLock = Volatile.Read(ref ulongLock);
                ref var lastLock = ref Unsafe.As<ulong, DataLock>(ref lastUlongLock);
                var newUlongLock = lastUlongLock;
                ref var newLock = ref Unsafe.As<ulong, DataLock>(ref newUlongLock);
                if (lastLock.Session != DataLockWrapExtensions.CurrentSession)
                {
                    newLock.Session = DataLockWrapExtensions.CurrentSession;
                    newLock.Thread = 0;
                    newLock.State = 0;
                    if (Interlocked.CompareExchange(ref ulongLock, newUlongLock, lastUlongLock) != lastUlongLock)
                    {
                        spinner.SpinOnce();
                    }

                    continue;
                }

                if (lastLock.Thread == DataLockWrapExtensions.CurrentThread)
                {
                    _isChild = true;
                    break;
                }

                if (lastLock.State < 0)
                {
                    spinner.SpinOnce();
                    continue;
                }

                newLock.Thread = DataLockWrapExtensions.CurrentThread;
                newLock.State = (short)~newLock.State;
                if (Interlocked.CompareExchange(ref ulongLock, newUlongLock, lastUlongLock) != lastUlongLock)
                {
                    spinner.SpinOnce();
                    continue;
                }

                while (Volatile.Read(ref @lock.State) != -1)
                    spinner.SpinOnce();

                break;
            }

            cancellationToken.ThrowIfCancellationRequested();

        }
        catch
        {
            _cache.Dispose();
            throw;
        }
    }

    public ref TValue RefValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            if (Volatile.Read(ref _pointer.Lock.State) != -1)
            {
                throw new ObjectDisposedException(nameof(DataLockWriteDisposable<>));
            }

            return ref _pointer;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        try
        {
            if (_isChild)
            {
                return;
            }

            ref var @lock = ref _pointer.Lock;
            ref var ulongLock = ref Unsafe.As<DataLock, ulong>(ref @lock);
            SpinWait spinner = default;
            while (!_cancellationToken.IsCancellationRequested)
            {
                var lastUlongLock = Volatile.Read(ref ulongLock);
                ref var lastLock = ref Unsafe.As<ulong, DataLock>(ref lastUlongLock);
                var newUlongLock = lastUlongLock;
                ref var newLock = ref Unsafe.As<ulong, DataLock>(ref newUlongLock);
                if (lastLock.State != -1)
                {
                    break;
                }

                newLock.Thread = 0;
                newLock.State = 0;
                if (Interlocked.CompareExchange(ref ulongLock, newUlongLock, lastUlongLock) != lastUlongLock)
                {
                    spinner.SpinOnce();
                    continue;
                }

                return;
            }

            throw new ObjectDisposedException(nameof(DataLockWriteDisposable<>));
        }
        finally
        {
            _cache.Cache.ExitAccess();
            _cache.Dispose();
        }
    }
}
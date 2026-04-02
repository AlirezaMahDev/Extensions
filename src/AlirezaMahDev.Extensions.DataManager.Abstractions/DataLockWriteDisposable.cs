namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Auto)]
public readonly ref struct DataLockWriteDisposable<TValue> : IDisposable
    where TValue : unmanaged, IDataValue<TValue>
{
    private readonly CancellationToken _cancellationToken;
    private readonly bool _isChild;
    private readonly ref readonly DataLocation<TValue> _location;

    public DataLockWriteDisposable(ref readonly DataLocation<TValue> location,
        CancellationToken cancellationToken = default)
    {
        _location = ref location;
        _cancellationToken = cancellationToken;

        ref var @lock = ref location.UnsafeRefValue.Lock;
        ref var ulongLock = ref Unsafe.As<DataLock, ulong>(ref @lock);
        SpinWait spinner = default;
        while (!cancellationToken.IsCancellationRequested)
        {
            var lastUlongLock = Volatile.Read(ref ulongLock);
            var newUlongLock = lastUlongLock;
            ref var newLock = ref Unsafe.As<ulong, DataLock>(ref newUlongLock);
            if (newLock.Session != DataLockWrapExtensions.CurrentSession)
            {
                newLock.Session = DataLockWrapExtensions.CurrentSession;
                newLock.Thread = 0;
                newLock.State = 0;
                if (Interlocked.CompareExchange(ref ulongLock, newUlongLock, lastUlongLock) != lastUlongLock)
                {
                    spinner.SpinOnce();
                    continue;
                }
            }

            if (newLock.Thread == DataLockWrapExtensions.CurrentThread)
            {
                _isChild = true;
                break;
            }

            if (newLock.State < 0)
            {
                spinner.SpinOnce();
                continue;
            }

            if (newLock.State >= 0)
            {
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
        }
    }

    public ref TValue RefValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            if (Volatile.Read(ref _location.UnsafeRefValue.Lock.State) != -1)
            {
                throw new ObjectDisposedException(nameof(DataLockWriteDisposable<>));
            }

            return ref _location.UnsafeRefValue;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (_isChild)
        {
            return;
        }

        ref var @lock = ref _location.UnsafeRefValue.Lock;
        ref var ulongLock = ref Unsafe.As<DataLock, ulong>(ref @lock);
        SpinWait spinner = default;
        while (!_cancellationToken.IsCancellationRequested)
        {
            var lastUlongLock = Volatile.Read(ref ulongLock);
            var newUlongLock = lastUlongLock;
            ref var newLock = ref Unsafe.As<ulong, DataLock>(ref newUlongLock);
            if (newLock.State != -1)
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
}
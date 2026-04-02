namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Auto)]
public readonly ref struct DataLockReadDisposable<TValue> : IDisposable
    where TValue : unmanaged, IDataValue<TValue>
{
    private readonly bool _isChild;
    private readonly CancellationToken _cancellationToken;
    private readonly ref readonly DataLocation<TValue> _location;
    private readonly ref readonly TValue _pointer;

    public DataLockReadDisposable(ref readonly DataLocation<TValue> location,
        CancellationToken cancellationToken = default)
    {
        _cancellationToken = cancellationToken;
        _location = ref location;

        ref var state = ref location.UnsafeRefValue.Lock;
        ref var ulongState = ref Unsafe.As<DataLock, ulong>(ref state);
        SpinWait spinner = default;
        while (!cancellationToken.IsCancellationRequested)
        {
            var lastUlongLock = Volatile.Read(ref ulongState);
            var newUlongLock = lastUlongLock;
            ref var newLock = ref Unsafe.As<ulong, DataLock>(ref newUlongLock);
            if (newLock.Session != DataLockWrapExtensions.CurrentSession)
            {
                newLock.Session = DataLockWrapExtensions.CurrentSession;
                newLock.Thread = 0;
                newLock.State = 0;
                if (Interlocked.CompareExchange(ref ulongState, newUlongLock, lastUlongLock) != lastUlongLock)
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
                newLock.State++;
                if (Interlocked.CompareExchange(ref ulongState, newUlongLock, lastUlongLock) != lastUlongLock)
                {
                    spinner.SpinOnce();
                    continue;
                }

                break;
            }
        }
        cancellationToken.ThrowIfCancellationRequested();
        _pointer = ref _location.UnsafeRefReadOnlyValue;
    }

    public ref readonly TValue RefReadOnlyValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            if (!_isChild && Volatile.Read(ref _location.UnsafeRefValue.Lock.State) is -1 or 0)
            {
                throw new ObjectDisposedException(nameof(DataLockReadDisposable<>));
            }

            return ref _pointer;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (_isChild)
        {
            return;
        }

        ref var state = ref _location.UnsafeRefValue.Lock.State;
        if (Volatile.Read(ref state) is -1 or 0)
        {
            throw new ObjectDisposedException(nameof(DataLockReadDisposable<>));
        }

        ref var @lock = ref _location.UnsafeRefValue.Lock;
        ref var ulongLock = ref Unsafe.As<DataLock, ulong>(ref @lock);
        SpinWait spinner = default;
        while (!_cancellationToken.IsCancellationRequested)
        {
            var lastUlongLock = Volatile.Read(ref ulongLock);
            var newUlongLock = lastUlongLock;
            ref var newLock = ref Unsafe.As<ulong, DataLock>(ref newUlongLock);
            if (newLock.State is 0 or -1)
            {
                break;
            }

            if (newLock.State > 0)
            {
                newLock.State--;
            }
            else
            {
                newLock.State++;
            }

            if (newLock.State == 0)
            {
                newLock.Thread = 0;
            }

            if (Interlocked.CompareExchange(ref ulongLock, newUlongLock, lastUlongLock) != lastUlongLock)
            {
                spinner.SpinOnce();
                continue;
            }

            return;
        }
    }
}
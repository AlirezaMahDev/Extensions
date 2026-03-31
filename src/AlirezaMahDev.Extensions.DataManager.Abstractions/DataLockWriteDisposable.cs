namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly ref struct DataLockWriteDisposable<TValue> : IDisposable
    where TValue : unmanaged, IDataValue<TValue>
{
    private readonly ref readonly DataLocation<TValue> _location;

    public DataLockWriteDisposable(ref readonly DataLocation<TValue> location, CancellationToken cancellationToken = default)
    {
        _location = ref location;

        ref var state = ref location.UnsafeRefValue.Lock.State;
        SpinWait spinner = default;
        while (!cancellationToken.IsCancellationRequested)
        {
            int lastState = Volatile.Read(ref state);

            if (lastState > 0)
            {
                Interlocked.CompareExchange(ref state,
                    ~lastState,
                    lastState);
                continue;
            }

            if (lastState != 0)
            {
                spinner.SpinOnce();
                continue;
            }

            if (Interlocked.CompareExchange(
                    ref state,
                    -1,
                    lastState) == lastState)
            {
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
                throw new ObjectDisposedException(nameof(DataLockReadDisposable<>));
            }

            return ref _location.UnsafeRefValue;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _location.UnsafeRefValue.Lock.State, 0, -1) == -1)
        {
            return;
        }

        throw new ObjectDisposedException(nameof(DataLockReadDisposable<>));
    }
}
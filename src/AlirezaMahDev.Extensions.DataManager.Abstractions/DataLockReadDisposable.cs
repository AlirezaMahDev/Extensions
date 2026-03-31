namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly ref struct DataLockReadDisposable<TValue> : IDisposable
    where TValue : unmanaged, IDataValue<TValue>
{
    private readonly ref readonly DataLocation<TValue> _location;

    public DataLockReadDisposable(ref readonly DataLocation<TValue> location, CancellationToken cancellationToken = default)
    {
        _location = ref location;
        ref var state = ref location.UnsafeRefValue.Lock.State;
        SpinWait spinner = default;
        while (!cancellationToken.IsCancellationRequested)
        {
            int lastState = Volatile.Read(ref state);
            if (lastState < 0)
            {
                spinner.SpinOnce();
                continue;
            }

            if (Interlocked.CompareExchange(
                    ref state,
                    lastState + 1,
                    lastState) == lastState)
            {
                break;
            }
        }
    }

    public readonly ref readonly TValue RefReadOnlyValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            if (Volatile.Read(ref _location.UnsafeRefValue.Lock.State) is -1 or 0)
            {
                throw new ObjectDisposedException(nameof(DataLockReadDisposable<>));
            }

            return ref _location.UnsafeRefValue;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (Volatile.Read(ref _location.UnsafeRefValue.Lock.State) is not -1 and not 0)
        {
            ref var state = ref _location.UnsafeRefValue.Lock.State;
            int lastState;
            int newState;
            do
            {
                lastState = Volatile.Read(ref state);
                newState = lastState > 0
                    ? lastState - 1
                    : lastState < -1
                        ? lastState + 1
                        : throw new ObjectDisposedException(nameof(DataLockReadDisposable<>));
            } while (Interlocked.CompareExchange(
                         ref state,
                         newState,
                         lastState) !=
                     lastState);
            return;
        }

        throw new ObjectDisposedException(nameof(DataLockReadDisposable<>));
    }
}
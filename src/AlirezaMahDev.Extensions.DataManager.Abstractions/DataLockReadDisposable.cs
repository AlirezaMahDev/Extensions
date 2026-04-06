using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[MustDisposeResource]
[StructLayout(LayoutKind.Auto)]
public readonly ref struct DataLockReadDisposable<TValue> : IDisposable
    where TValue : unmanaged, IDataValue<TValue>
{
    private readonly bool _isChild;
    private readonly CancellationToken _cancellationToken;
    private readonly ref readonly DataLocation<TValue> _location;
    private readonly DataMapFilePartCacheAccess _cache;
    private readonly ref TValue _pointer;

    public DataLockReadDisposable(ref readonly DataLocation<TValue> location,
        CancellationToken cancellationToken = default)
    {
        _cancellationToken = cancellationToken;
        _location = ref location;
        _cache = location.Access.GetCache(in _location.Offset, cancellationToken);
        try
        {
            _pointer = ref Unsafe.As<byte, TValue>(ref _cache.Cache.EnterAccessRefByte(_location.Offset.Offset));
            ref var state = ref _pointer.Lock;
            ref var ulongState = ref Unsafe.As<DataLock, ulong>(ref state);
            SpinWait spinner = default;
            while (!cancellationToken.IsCancellationRequested)
            {
                var lastUlongLock = Volatile.Read(ref ulongState);
                ref var lastLock = ref Unsafe.As<ulong, DataLock>(ref lastUlongLock);
                var newUlongLock = lastUlongLock;
                ref var newLock = ref Unsafe.As<ulong, DataLock>(ref newUlongLock);
                if (lastLock.Session != DataLockWrapExtensions.CurrentSession)
                {
                    newLock.Session = DataLockWrapExtensions.CurrentSession;
                    newLock.Thread = 0;
                    newLock.State = 0;
                    if (Interlocked.CompareExchange(ref ulongState, newUlongLock, lastUlongLock) != lastUlongLock)
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

                newLock.State++;
                if (Interlocked.CompareExchange(ref ulongState, newUlongLock, lastUlongLock) != lastUlongLock)
                {
                    spinner.SpinOnce();
                    continue;
                }

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

    public ref readonly TValue RefReadOnlyValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            if (!_isChild && Volatile.Read(ref _pointer.Lock.State) is -1 or 0)
            {
                throw new ObjectDisposedException(nameof(DataLockReadDisposable<>));
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
                if (lastLock.State is 0 or -1)
                {
                    break;
                }

                if (lastLock.State > 0)
                {
                    newLock.State--;
                }
                else
                {
                    newLock.State++;
                }

                if (Interlocked.CompareExchange(ref ulongLock, newUlongLock, lastUlongLock) != lastUlongLock)
                {
                    spinner.SpinOnce();
                    continue;
                }

                return;
            }

            throw new ObjectDisposedException(nameof(DataLockReadDisposable<>));
        }
        finally
        {
            _cache.Cache.ExitAccess();
            _cache.Dispose();
        }
    }
}
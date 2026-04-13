namespace AlirezaMahDev.Extensions.Abstractions;


public interface ILockerStatus
{
    bool IsFree { get; }
    int WriterId { get; }
    uint ReaderCount { get; }
}

public static class LockerStatusExtensions
{
    extension<TSelf>(TSelf self)
        where TSelf : ILockerStatus
    {
        public bool HasWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return self.WriterId != 0;
            }
        }

        public bool HasReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return self.ReaderCount > 0;
            }
        }
    }
}

[StructLayout(LayoutKind.Sequential, Size = 8, Pack = 1)]
public struct ReaderWriterLocker : ILockerStatus
{
    private long _state;

    public bool IsFree
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            var lastStateLong = Volatile.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            return lastState.WriterId == 0 && lastState.ReaderCount == 0;
        }
    }

    public int WriterId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            var lastStateLong = Volatile.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            return lastState.WriterId;
        }
    }

    public uint ReaderCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            var lastStateLong = Volatile.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            return lastState.ReaderCount;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryEnterReadLock(int timeout = -1, CancellationToken cancellationToken = default)
    {
        int maxRetry;
        if (timeout == 0)
        {
            maxRetry = 1;
            timeout = -1;
        }
        else
        {
            maxRetry = int.MaxValue;
        }

        int retry = 0;
        SpinWait spinWait = default;
        var start = Environment.TickCount64;
        do
        {
            var lastStateLong = Volatile.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            var newStateLong = lastStateLong;
            ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);
            if (lastState.WriterId != 0)
            {
                spinWait.SpinOnce();
                retry++;
                continue;
            }

            newState.ReaderCount++;

            if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
            {
                spinWait.SpinOnce();
                retry++;
                continue;
            }

            return true;
        } while (!cancellationToken.IsCancellationRequested && retry < maxRetry && (timeout < 0 || Environment.TickCount64 - start < timeout));
        cancellationToken.ThrowIfCancellationRequested();
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void TryExitReadLock(CancellationToken cancellationToken = default)
    {
        SpinWait spinWait = default;
        while (!cancellationToken.IsCancellationRequested)
        {
            var lastStateLong = Volatile.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            var newStateLong = lastStateLong;
            ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

            if (lastState.ReaderCount == 0)
            {
                throw new InvalidOperationException("No read lock to release");
            }

            newState.ReaderCount--;

            if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
            {
                spinWait.SpinOnce();
                continue;
            }

            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryEnterWriteLock(int timeout = -1, CancellationToken cancellationToken = default)
    {
        int maxRetry;
        if (timeout == 0)
        {
            maxRetry = 1;
            timeout = -1;
        }
        else
        {
            maxRetry = int.MaxValue;
        }

        int retry = 0;
        SpinWait spinWait = default;
        var start = Environment.TickCount64;
        int currentManagedThreadId = Environment.CurrentManagedThreadId;
        try
        {
            do
            {
                var lastStateLong = Volatile.Read(ref _state);
                ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
                var newStateLong = lastStateLong;
                ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

                if (lastState.WriterId != 0)
                {
                    spinWait.SpinOnce();
                    retry++;
                    continue;
                }

                newState.WriterId = currentManagedThreadId;

                if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
                {
                    spinWait.SpinOnce();
                    retry++;
                    continue;
                }

                break;
            } while (!cancellationToken.IsCancellationRequested && retry < maxRetry && (timeout < 0 || Environment.TickCount64 - start < timeout));

            cancellationToken.ThrowIfCancellationRequested();
            if (timeout >= 0 && Environment.TickCount64 - start > timeout)
            {
                while (true)
                {
                    var lastStateLong = Volatile.Read(ref _state);
                    ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
                    var newStateLong = lastStateLong;
                    ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

                    if (lastState.WriterId != currentManagedThreadId)
                    {
                        return false;
                    }

                    newState.WriterId = 0;

                    if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
                    {
                        spinWait.SpinOnce();
                        continue;
                    }
                    break;
                }
                return false;
            }

            do
            {

                var lastStateLong = Volatile.Read(ref _state);
                ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
                var newStateLong = lastStateLong;
                ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

                if (lastState.ReaderCount == 0)
                {
                    return true;
                }

                spinWait.SpinOnce();
                retry++;
            } while (!cancellationToken.IsCancellationRequested && retry < maxRetry && (timeout < 0 || Environment.TickCount64 - start < timeout));

            cancellationToken.ThrowIfCancellationRequested();
            return false;
        }
        catch
        {
            while (true)
            {
                var lastStateLong = Volatile.Read(ref _state);
                ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
                var newStateLong = lastStateLong;
                ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

                if (lastState.WriterId != currentManagedThreadId)
                {
                    return false;
                }

                newState.WriterId = 0;

                if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
                {
                    spinWait.SpinOnce();
                    continue;
                }
                break;
            }
            throw;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void TryExitWriteLock(CancellationToken cancellationToken = default)
    {
        SpinWait spinWait = default;
        while (!cancellationToken.IsCancellationRequested)
        {
            var lastStateLong = Volatile.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            var newStateLong = lastStateLong;
            ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);
            if (lastState.ReaderCount != 0)
            {
                throw new("you are not in write lock");
            }

            if (lastState.WriterId != Environment.CurrentManagedThreadId)
            {
                throw new("you are not in write lock on this thread");
            }

            newState.WriterId = 0;

            if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
            {
                spinWait.SpinOnce();
                continue;
            }

            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
    }
}


[StructLayout(LayoutKind.Sequential, Size = 8, Pack = 1)]
public struct ReaderWriterLockerState
{
    public int WriterId;
    public uint ReaderCount;
}

public static class ReaderWriterLockerExtensions
{
    extension(ref ReaderWriterLocker locker)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReaderWriterLockerReaderDispose EnterReadLockScope(CancellationToken cancellationToken = default)
        {
            locker.TryEnterReadLock(cancellationToken: cancellationToken);
            return new(ref locker, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReaderWriterLockerWriterDispose EnterWriteLockScope(CancellationToken cancellationToken = default)
        {
            locker.TryEnterWriteLock(cancellationToken: cancellationToken);
            return new(ref locker, cancellationToken);
        }
    }
}

public readonly ref struct ReaderWriterLockerWriterDispose : IDisposable
{
    private readonly ref ReaderWriterLocker _readerWriterLocker;
    private readonly CancellationToken _cancellationToken;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReaderWriterLockerWriterDispose(ref ReaderWriterLocker readerWriterLocker, CancellationToken cancellationToken = default)
    {
        _readerWriterLocker = ref readerWriterLocker;
        _cancellationToken = cancellationToken;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _readerWriterLocker.TryExitWriteLock(_cancellationToken);
    }
}

public readonly ref struct ReaderWriterLockerReaderDispose : IDisposable
{
    private readonly ref ReaderWriterLocker _readerWriterLocker;
    private readonly CancellationToken _cancellationToken;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReaderWriterLockerReaderDispose(ref ReaderWriterLocker readerWriterLocker, CancellationToken cancellationToken = default)
    {
        _readerWriterLocker = ref readerWriterLocker;
        _cancellationToken = cancellationToken;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _readerWriterLocker.TryExitReadLock(_cancellationToken);
    }
}

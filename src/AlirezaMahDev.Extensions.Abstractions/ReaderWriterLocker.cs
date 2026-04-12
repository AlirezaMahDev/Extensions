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
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public struct ReaderWriterLocker(bool recursion) : ILockerStatus
{
    private readonly bool _recursion = recursion;
    private long _state;

    public bool IsFree
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            var lastStateLong = Interlocked.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            return lastState.WriterId == 0 && lastState.ReaderCount == 0;
        }
    }

    public int WriterId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            var lastStateLong = Interlocked.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            return lastState.WriterId;
        }
    }

    public uint ReaderCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            var lastStateLong = Interlocked.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            return lastState.ReaderCount;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool EnterReadLock(int timeout = -1, CancellationToken cancellationToken = default)
    {
        SpinWait spinWait = default;
        var start = Environment.TickCount64;
        while (!cancellationToken.IsCancellationRequested && (timeout < 0 || Environment.TickCount64 - start < timeout))
        {
            var lastStateLong = Interlocked.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            var newStateLong = lastStateLong;
            ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);
            if (lastState.WriterId != 0)
            {
                spinWait.SpinOnce();
                continue;
            }

            newState.ReaderCount++;

            if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
            {
                spinWait.SpinOnce();
                continue;
            }

            return true;
        }

        cancellationToken.ThrowIfCancellationRequested();
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void ExitReadLock(CancellationToken cancellationToken = default)
    {
        SpinWait spinWait = default;
        while (!cancellationToken.IsCancellationRequested)
        {
            var lastStateLong = Interlocked.Read(ref _state);
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
    public bool EnterWriteLock(int timeout = -1, CancellationToken cancellationToken = default)
    {
        SpinWait spinWait = default;
        var start = Environment.TickCount64;
        int currentManagedThreadId = Environment.CurrentManagedThreadId;
        try
        {
            while (!cancellationToken.IsCancellationRequested && (timeout < 0 || Environment.TickCount64 - start < timeout))
            {
                var lastStateLong = Interlocked.Read(ref _state);
                ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
                var newStateLong = lastStateLong;
                ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

                if (lastState.WriterId != 0)
                {
                    spinWait.SpinOnce();
                    continue;
                }

                newState.WriterId = currentManagedThreadId;

                if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
                {
                    spinWait.SpinOnce();
                    continue;
                }

                break;
            }

            cancellationToken.ThrowIfCancellationRequested();

            while (!cancellationToken.IsCancellationRequested && (timeout < 0 || Environment.TickCount64 - start < timeout))
            {

                var lastStateLong = Interlocked.Read(ref _state);
                ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
                var newStateLong = lastStateLong;
                ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

                if (lastState.ReaderCount == 0)
                {
                    return true;
                }

                spinWait.SpinOnce();
            }

            cancellationToken.ThrowIfCancellationRequested();
            return false;
        }
        catch
        {
            while (true)
            {
                var lastStateLong = Interlocked.Read(ref _state);
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
    public void ExitWriteLock(CancellationToken cancellationToken = default)
    {
        SpinWait spinWait = default;
        while (!cancellationToken.IsCancellationRequested)
        {
            var lastStateLong = Interlocked.Read(ref _state);
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
        public ReaderWriterLockerReaderDispose EnterReadLockScope(int timeout = -1, CancellationToken cancellationToken = default)
        {
            return new(ref locker, timeout, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReaderWriterLockerWriterDispose EnterWriteLockScope(int timeout = -1, CancellationToken cancellationToken = default)
        {
            return new(ref locker, timeout, cancellationToken);
        }
    }
}

public readonly ref struct ReaderWriterLockerWriterDispose : IDisposable
{
    private readonly ref ReaderWriterLocker _readerWriterLocker;
    private readonly CancellationToken _cancellationToken;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReaderWriterLockerWriterDispose(ref ReaderWriterLocker readerWriterLocker, int timeout = -1, CancellationToken cancellationToken = default)
    {
        _readerWriterLocker = ref readerWriterLocker;
        _cancellationToken = cancellationToken;
        _readerWriterLocker.EnterWriteLock(timeout, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _readerWriterLocker.ExitWriteLock(_cancellationToken);
    }
}

public readonly ref struct ReaderWriterLockerReaderDispose : IDisposable
{
    private readonly ref ReaderWriterLocker _readerWriterLocker;
    private readonly CancellationToken _cancellationToken;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReaderWriterLockerReaderDispose(ref ReaderWriterLocker readerWriterLocker, int timeout = -1, CancellationToken cancellationToken = default)
    {
        _readerWriterLocker = ref readerWriterLocker;
        _cancellationToken = cancellationToken;
        _readerWriterLocker.EnterReadLock(timeout, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _readerWriterLocker.ExitReadLock(_cancellationToken);
    }
}

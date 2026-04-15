namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockerStatus
{
    ReaderWriterLockerState LockerState { get; }
}

public static class LockerStatusExtensions
{
    extension<TSelf>(TSelf self)
        where TSelf : ILockerStatus
    {
        public bool IsFree
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                var state = self.LockerState;
                return state.ThreadId == Environment.CurrentManagedThreadId || state is { ThreadId: 0, ReaderCount: 0, WriterCount: 0 };
            }
        }

        public int ThreadId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return self.LockerState.ThreadId;
            }
        }

        public int WriterCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return self.LockerState.WriterCount is var writerCount && writerCount < 0 ? -writerCount : writerCount;
            }
        }
        public bool HasWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return self.WriterCount > 0;
            }
        }

        public int ReaderCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return self.LockerState.ReaderCount;
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
    private static readonly ThreadLocal<MemoryValue<int>> HasReader = new(() => new(0));

    public ReaderWriterLockerState LockerState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            var lastStateLong = Volatile.Read(ref _state);
            return Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
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
        var startTime = Environment.TickCount64;
        do
        {
            var lastStateLong = Volatile.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            var newStateLong = lastStateLong;
            ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

            if (lastState.ThreadId != 0 && (lastState.ThreadId != Environment.CurrentManagedThreadId || lastState.WriterCount < 0))
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

            if (lastState.ThreadId != Environment.CurrentManagedThreadId)
            {
                Interlocked.Increment(ref HasReader.Value.Value);
            }

            return true;
        } while (!cancellationToken.IsCancellationRequested && retry < maxRetry && (timeout < 0 || Environment.TickCount64 - startTime < timeout));
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

            if (lastState.WriterCount > 0 && lastState.ThreadId != Environment.CurrentManagedThreadId)
            {
                throw new InvalidOperationException("Cannot release read lock while another thread holds an active write lock.");
            }

            newState.ReaderCount--;

            if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
            {
                spinWait.SpinOnce();
                continue;
            }

            if (lastState.ThreadId != Environment.CurrentManagedThreadId)
            {
                Interlocked.Decrement(ref HasReader.Value.Value);
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
        long startTime = Environment.TickCount64;
        int currentManagedThreadId = Environment.CurrentManagedThreadId;
        try
        {
            switch (TrySetCurrentThread(currentManagedThreadId, ref spinWait, startTime, timeout, ref retry, maxRetry, cancellationToken))
            {
                case true:
                    if (TryWaitExitAllReaderAndSetWriterOne(ref spinWait, startTime, timeout, ref retry, maxRetry, cancellationToken))
                    {
                        return true;
                    }
                    else
                    {
                        ResetThreadId(currentManagedThreadId, ref spinWait);
                        return false;
                    }
                case null:
                    return TryIncreaseWriterCount(ref spinWait, startTime, timeout, ref retry, maxRetry, cancellationToken);
                case false:
                    return false;
            }
        }
        catch
        {
            ResetThreadId(currentManagedThreadId, ref spinWait);
            throw;
        }
    }

    private bool TryIncreaseWriterCount(ref SpinWait spinWait, long startTime, long timeout, ref int retry, int maxRetry, CancellationToken cancellationToken = default)
    {
        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lastStateLong = Volatile.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            var newStateLong = lastStateLong;
            ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

            newState.WriterCount++;

            if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
            {
                spinWait.SpinOnce();
                retry++;
                continue;
            }

            return true;
        } while (retry < maxRetry && (timeout < 0 || Environment.TickCount64 - startTime < timeout));
        return false;
    }

    private bool? TrySetCurrentThread(int currentManagedThreadId, ref SpinWait spinWait, long startTime, long timeout, ref int retry, int maxRetry, CancellationToken cancellationToken = default)
    {
        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lastStateLong = Volatile.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            var newStateLong = lastStateLong;
            ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

            if (Volatile.Read(ref HasReader.Value.Value) != 0)
            {
                throw new InvalidOperationException("Cannot enter write lock because the current thread already holds a read lock (Upgrade is not supported). Release read lock first.");
            }

            if (lastState.ThreadId == currentManagedThreadId)
            {
                if (lastState.WriterCount < 0)
                {
                    spinWait.SpinOnce();
                    retry++;
                    continue;
                }
                else
                {
                    return null;
                }
            }

            if (lastState.ThreadId != 0)
            {
                spinWait.SpinOnce();
                retry++;
                continue;
            }

            newState.ThreadId = currentManagedThreadId;
            newState.WriterCount = -1;

            if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
            {
                spinWait.SpinOnce();
                retry++;
                continue;
            }

            return true;
        } while (retry < maxRetry && (timeout < 0 || Environment.TickCount64 - startTime < timeout));
        return false;
    }

    private bool TryWaitExitAllReaderAndSetWriterOne(ref SpinWait spinWait, long startTime, long timeout, ref int retry, int maxRetry, CancellationToken cancellationToken = default)
    {
        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lastStateLong = Volatile.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            var newStateLong = lastStateLong;
            ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

            if (lastState.ReaderCount != 0)
            {
                spinWait.SpinOnce();
                retry++;
                continue;
            }

            newState.WriterCount = 1;

            if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
            {
                spinWait.SpinOnce();
                retry++;
                continue;
            }

            return true;
        } while (retry < maxRetry && (timeout < 0 || Environment.TickCount64 - startTime < timeout));
        return false;
    }

    private void ResetThreadId(int currentManagedThreadId, ref SpinWait spinWait)
    {
        while (true)
        {
            var lastStateLong = Volatile.Read(ref _state);
            ref var lastState = ref Unsafe.As<long, ReaderWriterLockerState>(ref lastStateLong);
            var newStateLong = lastStateLong;
            ref var newState = ref Unsafe.As<long, ReaderWriterLockerState>(ref newStateLong);

            if (lastState.ThreadId != currentManagedThreadId)
            {
                break;
            }

            newState.ThreadId = 0;
            newState.WriterCount = 0;

            if (Interlocked.CompareExchange(ref _state, newStateLong, lastStateLong) != lastStateLong)
            {
                spinWait.SpinOnce();
                continue;
            }

            break;
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

            if (lastState.WriterCount == 0 || lastState.ThreadId == 0)
            {
                throw new InvalidOperationException("you are not in write lock");
            }

            if (lastState.ThreadId != Environment.CurrentManagedThreadId)
            {
                throw new InvalidOperationException("you are not in write lock on this thread");
            }

            if (lastState.WriterCount == 1 && lastState.ReaderCount > 0)
            {
                throw new InvalidOperationException("Cannot exit write lock while holding a nested read lock. Release read lock first.");
            }

            newState.WriterCount--;
            if (newState.WriterCount == 0)
            {
                newState.ThreadId = 0;
            }

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
    public int ThreadId;
    public short WriterCount;
    public ushort ReaderCount;
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

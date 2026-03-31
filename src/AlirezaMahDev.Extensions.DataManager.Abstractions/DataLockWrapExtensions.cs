using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLockWrapExtensions
{
    private static readonly uint CurrentSession =
        XxHash32.HashToUInt32(Guid.NewGuid().ToByteArray()) | 1;

    extension<TValue>(in DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private void FreeLastLock()
        {
            ref var lastSession = ref Unsafe.As<DataLock, ulong>(ref location.UnsafeRefValue.Lock);
            ulong currentSession = CurrentSession;
            var read = Volatile.Read(ref lastSession);
            if (read != 0 && (read >> 32) != CurrentSession)
            {
                Interlocked.CompareExchange(
                    ref lastSession,
                    currentSession << 32,
                    read);
            }
        }

        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLockWriteDisposable<TValue> WriteLock(CancellationToken cancellationToken = default)
        {
            location.FreeLastLock();
            return new(in location, cancellationToken);
        }

        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLockReadDisposable<TValue> ReadLock(CancellationToken cancellationToken = default)
        {
            location.FreeLastLock();
            return new(in location, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void WriteLock(DataLockScopedRefAction<TValue> action, CancellationToken cancellationToken = default)
        {
            using var lockScope = location.WriteLock(cancellationToken);
            action(ref lockScope.RefValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void WriteLock<TState>(DataLockScopedRefAction<TValue, TState> action, scoped in TState state)
        {
            using var lockScope = location.WriteLock(CancellationToken.None);
            action(ref lockScope.RefValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void WriteLock<TState>(DataLockScopedRefAction<TValue, TState> action, scoped in TState state, CancellationToken cancellationToken)
        {
            using var lockScope = location.WriteLock(cancellationToken);
            action(ref lockScope.RefValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult WriteLock<TResult>(DataLockScopedRefFunc<TValue, TResult> func, CancellationToken cancellationToken = default)
        {
            using var lockScope = location.WriteLock(cancellationToken);
            return func(ref lockScope.RefValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult WriteLock<TState, TResult>(DataLockScopedRefFunc<TValue, TState, TResult> func, scoped in TState state)
        {
            using var lockScope = location.WriteLock(CancellationToken.None);
            return func(ref lockScope.RefValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult WriteLock<TState, TResult>(DataLockScopedRefFunc<TValue, TState, TResult> func, scoped in TState state, CancellationToken cancellationToken)
        {
            using var lockScope = location.WriteLock(cancellationToken);
            return func(ref lockScope.RefValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void ReadLock(DataLockScopedRefReadonlyAction<TValue> action, CancellationToken cancellationToken = default)
        {
            using var lockScope = location.ReadLock(cancellationToken);
            action(in lockScope.RefReadOnlyValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void ReadLock<TState>(DataLockScopedRefReadonlyAction<TValue, TState> action, scoped in TState state)
        {
            using var lockScope = location.ReadLock(CancellationToken.None);
            action(in lockScope.RefReadOnlyValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void ReadLock<TState>(DataLockScopedRefReadonlyAction<TValue, TState> action, scoped in TState state, CancellationToken cancellationToken)
        {
            using var lockScope = location.ReadLock(cancellationToken);
            action(in lockScope.RefReadOnlyValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult ReadLock<TResult>(DataLockScopedRefReadonlyFunc<TValue, TResult> func, CancellationToken cancellationToken = default)
        {
            using var lockScope = location.ReadLock(cancellationToken);
            return func(in lockScope.RefReadOnlyValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult ReadLock<TState, TResult>(DataLockScopedRefReadonlyFunc<TValue, TState, TResult> func, scoped in TState state)
        {
            using var lockScope = location.ReadLock(CancellationToken.None);
            return func(in lockScope.RefReadOnlyValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult ReadLock<TState, TResult>(DataLockScopedRefReadonlyFunc<TValue, TState, TResult> func, scoped in TState state, CancellationToken cancellationToken)
        {
            using var lockScope = location.ReadLock(cancellationToken);
            return func(in lockScope.RefReadOnlyValue, in state);
        }
    }
}

public delegate void DataLockScopedRefReadonlyAction<TValue, TState>(scoped ref readonly TValue value, scoped in TState state)
    where TValue : unmanaged, IDataValue<TValue>;

public delegate void DataLockScopedRefAction<TValue, TState>(scoped ref TValue value, scoped in TState state)
    where TValue : unmanaged, IDataValue<TValue>;

public delegate TResult DataLockScopedRefReadonlyFunc<TValue, TState, out TResult>(scoped ref readonly TValue value, scoped in TState state)
    where TValue : unmanaged, IDataValue<TValue>;

public delegate TResult DataLockScopedRefFunc<TValue, TState, out TResult>(scoped ref TValue value, scoped in TState state)
    where TValue : unmanaged, IDataValue<TValue>;
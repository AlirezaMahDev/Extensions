using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLockWrapExtensions
{
    public static readonly ushort CurrentSession =
        (ushort)(XxHash32.HashToUInt32(Guid.NewGuid().ToByteArray()) | 1);

    public static int CurrentThread
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Environment.CurrentManagedThreadId;
    }

    extension<TValue>(in DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLockWriteDisposable<TValue> WriteLock(CancellationToken cancellationToken = default)
        {
            return new(in location, cancellationToken);
        }

        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLockReadDisposable<TValue> ReadLock(CancellationToken cancellationToken = default)
        {
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
        public void WriteLock<TState>(DataLockScopedRefAction<TValue, TState> action,
            scoped in TState state,
            CancellationToken cancellationToken)
        {
            using var lockScope = location.WriteLock(cancellationToken);
            action(ref lockScope.RefValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult WriteLock<TResult>(DataLockScopedRefFunc<TValue, TResult> func,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = location.WriteLock(cancellationToken);
            return func(ref lockScope.RefValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult WriteLock<TState, TResult>(DataLockScopedRefFunc<TValue, TState, TResult> func,
            scoped in TState state)
        {
            using var lockScope = location.WriteLock(CancellationToken.None);
            return func(ref lockScope.RefValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult WriteLock<TState, TResult>(DataLockScopedRefFunc<TValue, TState, TResult> func,
            scoped in TState state,
            CancellationToken cancellationToken)
        {
            using var lockScope = location.WriteLock(cancellationToken);
            return func(ref lockScope.RefValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void ReadLock(ScopedRefReadOnlyAction<TValue> action,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = location.ReadLock(cancellationToken: cancellationToken);
            action(in lockScope.RefReadOnlyValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void ReadLock<TState>(ScopedRefReadOnlyAction<TValue, TState> action, scoped in TState state)
        {
            using var lockScope = location.ReadLock(cancellationToken: CancellationToken.None);
            action(in lockScope.RefReadOnlyValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void ReadLock<TState>(ScopedRefReadOnlyAction<TValue, TState> action,
            scoped in TState state,
            CancellationToken cancellationToken)
        {
            using var lockScope = location.ReadLock(cancellationToken: cancellationToken);
            action(in lockScope.RefReadOnlyValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult ReadLock<TResult>(ScopedRefReadOnlyFunc<TValue, TResult> func,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = location.ReadLock(cancellationToken: cancellationToken);
            return func(in lockScope.RefReadOnlyValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult ReadLock<TState, TResult>(ScopedRefReadOnlyFunc<TValue, TState, TResult> func,
            scoped in TState state)
        {
            using var lockScope = location.ReadLock(cancellationToken: CancellationToken.None);
            return func(in lockScope.RefReadOnlyValue, in state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult ReadLock<TState, TResult>(ScopedRefReadOnlyFunc<TValue, TState, TResult> func,
            scoped in TState state,
            CancellationToken cancellationToken)
        {
            using var lockScope = location.ReadLock(cancellationToken: cancellationToken);
            return func(in lockScope.RefReadOnlyValue, in state);
        }
    }
}
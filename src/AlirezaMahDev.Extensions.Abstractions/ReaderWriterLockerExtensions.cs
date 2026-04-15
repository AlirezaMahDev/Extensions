namespace AlirezaMahDev.Extensions.Abstractions;

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
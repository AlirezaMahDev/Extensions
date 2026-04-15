namespace AlirezaMahDev.Extensions.Abstractions;

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
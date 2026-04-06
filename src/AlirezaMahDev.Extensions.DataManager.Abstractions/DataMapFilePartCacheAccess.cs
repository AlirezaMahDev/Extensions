namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly ref struct DataMapFilePartCacheAccess(
    IDataMapFilePartCache cache,
    ReaderWriterLockSlim readerWriterLockSlime) : IDisposable
{
    public IDataMapFilePartCache Cache { get; } = cache;

    public void Dispose()
    {
        readerWriterLockSlime.ExitReadLock();
    }
}
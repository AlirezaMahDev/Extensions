namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataLockOffset(DataLock dataLock, long offset) : IDisposable
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public DataLockOffsetDisposable Lock()
    {
        _semaphoreSlim.Wait();
        return new DataLockOffsetDisposable(this);
    }

    public async Task<DataLockOffsetDisposable> LockAsync(CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        return new DataLockOffsetDisposable(this);
    }

    public void Unlock()
    {
        _semaphoreSlim.Release();
    }

    public void Dispose()
    {
        _semaphoreSlim.Dispose();
        GC.SuppressFinalize(this);
    }
}

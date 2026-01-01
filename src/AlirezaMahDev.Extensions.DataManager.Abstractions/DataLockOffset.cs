namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataLockOffset(DataLock dataLock, long offset) : IDisposable
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public int CurrentCount => _semaphoreSlim.CurrentCount;


    public DataLockOffsetDisposable Lock()
    {
        _semaphoreSlim.Wait();
        return new(this);
    }

    public async Task<DataLockOffsetDisposable> LockAsync(CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        return new(this);
    }

    public void Unlock()
    {
        _semaphoreSlim.Release();
        if (_semaphoreSlim.CurrentCount == 0)
        {
            dataLock.Remove(offset);
        }
    }

    public void Dispose()
    {
        _semaphoreSlim.Dispose();
        GC.SuppressFinalize(this);
    }
}

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly struct LockScope(SemaphoreSlim semaphoreSlim) : IDisposable
{
    public void Dispose()
    {
        semaphoreSlim.Release();
    }
}
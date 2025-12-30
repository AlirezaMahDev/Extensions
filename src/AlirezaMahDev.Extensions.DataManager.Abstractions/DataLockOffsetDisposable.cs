namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly struct DataLockOffsetDisposable(DataLockOffset dataLockOffset) : IDisposable
{
    public void Dispose()
    {
        dataLockOffset.Unlock();
    }
}
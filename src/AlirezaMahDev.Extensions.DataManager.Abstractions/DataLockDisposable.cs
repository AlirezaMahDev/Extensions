namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly struct DataLockDisposable<TValue>(DataWrap<TValue> wrap) : IDisposable
    where TValue : unmanaged, IDataLock<TValue>
{
    public void Dispose()
    {
        wrap.UnLock();
    }
}
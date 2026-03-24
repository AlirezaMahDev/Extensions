namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly ref struct DataLockDisposable<TValue>(ref readonly DataWrap<TValue> wrap) : IDisposable
    where TValue : unmanaged, IDataLock<TValue>
{
    private readonly DataWrap<TValue> _wrap =  wrap;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _wrap.UnLock();
    }
}
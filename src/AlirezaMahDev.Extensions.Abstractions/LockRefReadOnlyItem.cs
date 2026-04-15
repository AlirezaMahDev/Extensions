namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct LockRefReadOnlyItem<T>(ref readonly T value, ReaderWriterLockerReaderDispose dispose) : IDisposable
{
    private readonly ReaderWriterLockerReaderDispose _dispose = dispose;

    public ref readonly T Value = ref value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly void Dispose()
    {
        _dispose.Dispose();
    }
}
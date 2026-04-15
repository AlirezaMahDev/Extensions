namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct LockRefItem<T>(ref T value, ReaderWriterLockerReaderDispose dispose) : IDisposable
{
    private readonly ReaderWriterLockerReaderDispose _dispose = dispose;

    public ref T Value = ref value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly void Dispose()
    {
        _dispose.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LockRefReadOnlyItem<T>(LockRefItem<T> lockRefItem) =>
        new(ref lockRefItem.Value, lockRefItem._dispose);
}
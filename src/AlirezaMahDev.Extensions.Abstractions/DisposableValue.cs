namespace AlirezaMahDev.Extensions.Abstractions;

[MustDisposeResource]
[StructLayout(LayoutKind.Auto)]
public ref struct DisposableValue<TValue, TDisposable>(TValue value, TDisposable disposable)
    where TDisposable : IDisposable
{
    private TDisposable _disposable = disposable;
    private bool _disposed = false;

    public TValue Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposable.Dispose();
            _disposed = true;
        }
    }
}
namespace AlirezaMahDev.Extensions.Abstractions;

[MustDisposeResource]
public readonly struct Disposable(Action action) : IDisposable
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        action();
    }
}
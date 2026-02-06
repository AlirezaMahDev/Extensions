using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

[MustDisposeResource]
public readonly struct Disposable(Action action) : IDisposable
{
    public void Dispose()
    {
        action();
    }
}

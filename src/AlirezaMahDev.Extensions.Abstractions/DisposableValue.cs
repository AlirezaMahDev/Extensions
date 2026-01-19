using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

[MustDisposeResource]
public readonly struct DisposableValue<TValue>(TValue value, IDisposable disposable) : IDisposable
{
    public TValue Value { get; } = value;

    public void Dispose()
    {
        disposable.Dispose();
    }
}
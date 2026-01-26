using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

[MustDisposeResource]
[StructLayout(LayoutKind.Auto)]
public readonly struct DisposableValue<TValue>(TValue value, IDisposable disposable) : IDisposable
{
    public TValue Value { get; } = value;

    public void Dispose()
    {
        disposable.Dispose();
    }
}
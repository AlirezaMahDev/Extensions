using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct ReadOnlyMemoryValue<T>
    where T : struct
{
    private readonly ReadOnlyMemory<T> _readOnlyMemory;

    public ReadOnlyMemoryValue(in T value)
    {
        _readOnlyMemory = new([value]);
    }

    public ReadOnlyMemoryValue(in ReadOnlyMemory<T> memory)
    {
        _readOnlyMemory = memory[..1];
    }

    public ref readonly T Value => ref _readOnlyMemory.Span[0];

    public static implicit operator ReadOnlyMemoryValue<T>(in T value) =>
        new(in value);

    public static implicit operator ReadOnlyMemory<T>(in ReadOnlyMemoryValue<T> memoryValue) =>
        memoryValue._readOnlyMemory;
    public static implicit operator ReadOnlySpanValue<T>(ReadOnlyMemoryValue<T> readOnlyMemory) => new(in readOnlyMemory.Value);

}
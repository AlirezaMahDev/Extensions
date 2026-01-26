using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct ReadOnlyMemoryValue<T>
{
    public ReadOnlyMemoryValue(in T value)
    {
        _memory = new([value]);
    }

    internal ReadOnlyMemoryValue(in ReadOnlyMemory<T> memory)
    {
        _memory = memory;
    }

    private readonly ReadOnlyMemory<T> _memory;

    public ref readonly T Value => ref _memory.Span[0];

    public static implicit operator ReadOnlyMemoryValue<T>(in T value) => new(in value);
    public static implicit operator ReadOnlyMemoryValue<T>(in MemoryValue<T> memoryValue) => new(memoryValue.Memory);
}
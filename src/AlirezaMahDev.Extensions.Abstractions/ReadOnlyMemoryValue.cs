namespace AlirezaMahDev.Extensions.Abstractions;

public readonly struct ReadOnlyMemoryValue<T>
{
    public ReadOnlyMemoryValue(in T value)
    {
        Memory = new([value]);
    }

    internal ReadOnlyMemoryValue(in ReadOnlyMemory<T> memory)
    {
        Memory = memory;
    }

    internal readonly ReadOnlyMemory<T> Memory;

    public ref readonly T Value => ref Memory.Span[0];

    public static implicit operator ReadOnlyMemoryValue<T>(in T value) => new(in value);
    public static implicit operator ReadOnlyMemoryValue<T>(in MemoryValue<T> memoryValue) => new(memoryValue.Memory);
}
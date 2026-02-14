namespace AlirezaMahDev.Extensions.Abstractions;

public readonly struct MemoryValue<T>
    where T : struct
{
    private readonly Memory<T> _memory;

    public MemoryValue(in T value)
    {
        _memory = new([value]);
    }

    public MemoryValue(in Memory<T> memory)
    {
        _memory = memory[..1];
    }

    public ref T Value => ref _memory.Span[0];

    public static implicit operator MemoryValue<T>(in T value) =>
        new(in value);

    public static implicit operator Memory<T>(in MemoryValue<T> memoryValue) =>
        memoryValue._memory;

    public static implicit operator ReadOnlyMemory<T>(in MemoryValue<T> memoryValue) =>
        memoryValue._memory;

    public static implicit operator ReadOnlyMemoryValue<T>(in MemoryValue<T> memoryValue) =>
        new(memoryValue);

    public static implicit operator SpanValue<T>(MemoryValue<T> memoryValue) => new(ref memoryValue.Value);
    public static implicit operator ReadOnlySpanValue<T>(MemoryValue<T> memoryValue) => new(in memoryValue.Value);
}
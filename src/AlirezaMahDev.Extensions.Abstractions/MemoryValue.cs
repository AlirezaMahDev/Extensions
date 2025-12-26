namespace AlirezaMahDev.Extensions.Abstractions;

public readonly struct MemoryValue<T>
{
    internal readonly Memory<T> Memory;

    public MemoryValue(in T value)
    {
        Memory = new([value]);
    }

    internal MemoryValue(in Memory<T> memory)
    {
        Memory = memory;
    }


    public ref T Value => ref Memory.Span[0];

    public static implicit operator MemoryValue<T>(T value) => new(value);
}
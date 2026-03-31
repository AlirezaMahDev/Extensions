namespace AlirezaMahDev.Extensions.Abstractions;

public readonly struct MemoryValue<T>
    where T : struct
{
    private readonly Memory<T> _memory;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public MemoryValue(T value)
    {
        _memory = new([value]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public MemoryValue(Memory<T> memory)
    {
        _memory = memory[..1];
    }

    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => !_memory.IsEmpty;
    }

    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _memory.Span[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator MemoryValue<T>(T value)
    {
        return new( value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator Memory<T>(MemoryValue<T> memoryValue)
    {
        return memoryValue._memory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlyMemory<T>(MemoryValue<T> memoryValue)
    {
        return memoryValue._memory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlyMemoryValue<T>(MemoryValue<T> memoryValue)
    {
        return new(memoryValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator SpanValue<T>(MemoryValue<T> memoryValue)
    {
        return new(ref memoryValue.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlySpanValue<T>(MemoryValue<T> memoryValue)
    {
        return new(ref memoryValue.Value);
    }
}
namespace AlirezaMahDev.Extensions.Abstractions;

public readonly struct MemoryValue<T>
    where T : struct
{
    private readonly Memory<T> _memory;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public MemoryValue(in T value)
    {
        _memory = new([value]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public MemoryValue(in Memory<T> memory)
    {
        _memory = memory[..1];
    }

    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return !_memory.IsEmpty;
        }
    }

    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _memory.Span[0];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator MemoryValue<T>(in T value)
    {
        return new(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator Memory<T>(in MemoryValue<T> memoryValue)
    {
        return memoryValue._memory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlyMemory<T>(in MemoryValue<T> memoryValue)
    {
        return memoryValue._memory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlyMemoryValue<T>(in MemoryValue<T> memoryValue)
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
        return new(in memoryValue.Value);
    }
}
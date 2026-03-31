namespace AlirezaMahDev.Extensions.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct ReadOnlyMemoryValue<T>
    where T : struct
{
    private readonly ReadOnlyMemory<T> _readOnlyMemory;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReadOnlyMemoryValue(T value)
    {
        _readOnlyMemory = new([value]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReadOnlyMemoryValue(ReadOnlyMemory<T> memory)
    {
        _readOnlyMemory = memory[..1];
    }

    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => !_readOnlyMemory.IsEmpty;
    }

    public ref readonly T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _readOnlyMemory.Span[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlyMemoryValue<T>(T value)
    {
        return new(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlyMemory<T>(ReadOnlyMemoryValue<T> memoryValue)
    {
        return memoryValue._readOnlyMemory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlySpanValue<T>(ReadOnlyMemoryValue<T> readOnlyMemory)
    {
        return new(in readOnlyMemory.Value);
    }
}
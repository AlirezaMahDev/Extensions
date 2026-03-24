namespace AlirezaMahDev.Extensions.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct ReadOnlySpanValue<T>
    where T : struct
{
    private readonly ReadOnlySpan<T> _readOnlySpan;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReadOnlySpanValue(in T value)
    {
        _readOnlySpan = MemoryMarshal.CreateReadOnlySpan(in value, 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ReadOnlySpanValue(ReadOnlySpan<T> readOnlySpan)
    {
        _readOnlySpan = readOnlySpan[..1];
    }

    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return !_readOnlySpan.IsEmpty;
        }
    }

    public ref readonly T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref MemoryMarshal.GetReference(_readOnlySpan);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlySpanValue<T>(in T value)
    {
        return new(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ReadOnlySpan<T>(ReadOnlySpanValue<T> readOnlySpanValue)
    {
        return readOnlySpanValue._readOnlySpan;
    }
}
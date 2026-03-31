namespace AlirezaMahDev.Extensions.Abstractions;

public struct Optional<TValue>
    where TValue : struct, IEquatable<TValue>
{
    private TValue _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Optional(TValue value)
    {
        _value = value;
        HasValue = true;
    }

    public static Optional<TValue> Null
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Optional<TValue> From(TValue value)
    {
        return new(value);
    }

    public static implicit operator Optional<TValue>(TValue value)
    {
        return new(value);
    }

    public static implicit operator Optional<TValue>(TValue? value)
    {
        return value is null ? Null : From(value.Value);
    }

    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    public readonly ref TValue Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._value;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Optional<TValue> AsOptional()
    {
        return HasValue
            ? Optional<TValue>.From(Value)
            : Optional<TValue>.Null;
    }
}
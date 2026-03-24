namespace AlirezaMahDev.Extensions.Abstractions;

public struct Optional<TValue>
    where TValue : struct, IInEquatable<TValue>
{
    private TValue _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Optional(in TValue value)
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
    public static Optional<TValue> From(TValue value) => new(in value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Optional<TValue> From(in TValue value) => new(in value);

    public static implicit operator Optional<TValue>(in TValue value) => new(in value);

    public static implicit operator Optional<TValue>(in TValue? value) =>
        value is null ? Null : From(value.Value);

    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    public ref TValue Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Unsafe.AsRef(in this)._value;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefOptional<TValue> AsRefOptional()
        => HasValue
            ? RefOptional<TValue>.From(ref Value)
            : RefOptional<TValue>.Null;
}
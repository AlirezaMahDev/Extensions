namespace AlirezaMahDev.Extensions.Abstractions;

public readonly ref struct RefOptional<TValue>
    where TValue : struct, IInEquatable<TValue>
{
    private readonly ref TValue _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal RefOptional(ref TValue value)
    {
        _value = ref value;
    }

    public static RefOptional<TValue> Null
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return new(ref Unsafe.NullRef<TValue>());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static RefOptional<TValue> From(ref TValue value)
    {
        return new(ref value);
    }

    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return !Unsafe.IsNullRef(ref _value);
        }
    }

    public ref TValue Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Optional<TValue> AsOptional()
        => HasValue
            ? Optional<TValue>.From(in Value)
            : Optional<TValue>.Null;
}
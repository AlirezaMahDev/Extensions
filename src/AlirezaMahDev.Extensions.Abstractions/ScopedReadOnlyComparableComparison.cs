namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly ref struct ScopedReadOnlyComparableComparison<T>(T value, ScopedRefReadOnlyComparison<T> readOnlyComparison)
    : IScopedRefReadOnlyComparable<T>
    where T : allows ref struct
{
    private readonly T _value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(scoped ref readonly T other)
    {
        return other is null ? 1 : readOnlyComparison(in _value, in other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(T? other)
    {
        return other is null ? -1 : CompareTo(ref other);
    }
}
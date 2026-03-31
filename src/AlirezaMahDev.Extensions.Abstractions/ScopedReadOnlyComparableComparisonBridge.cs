namespace AlirezaMahDev.Extensions.Abstractions;

public readonly ref struct ScopedReadOnlyComparableComparisonBridge<T, TBridge>(
    TBridge value,
    ScopedRefReadOnlyFunc<T, TBridge> func,
    ScopedRefReadOnlyComparison<TBridge> readOnlyComparison) : IScopedRefReadOnlyComparable<T>
    where TBridge : allows ref struct
{
    private readonly TBridge _value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(scoped ref readonly T other)
    {
        if (other is null)
        {
            return 1;
        }

        scoped var bridge = func(in other);
        return readOnlyComparison(in _value, ref bridge);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(T? other)
    {
        return other is null ? -1 : CompareTo(ref other);
    }
}
namespace AlirezaMahDev.Extensions.Abstractions;

public readonly ref struct ScopedReadOnlyComparableComparerBridge<T, TBridge, TComparer>(
    TBridge value,
    ScopedRefReadOnlyFunc<T, TBridge> func,
    TComparer comparer)
    : IScopedRefReadOnlyComparable<T>
    where TComparer : IScopedRefReadOnlyComparer<TBridge>, allows ref struct
    where TBridge : allows ref struct
    where T : allows ref struct
{
    private readonly TBridge _value = value;
    private readonly TComparer _comparer = comparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(scoped ref readonly T other)
    {
        if (other is null)
        {
            return 1;
        }

        var bridge = func(in other);
        var value = _value;
        var refReadOnlyComparer = _comparer;
        return refReadOnlyComparer.Compare(in value, in bridge);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(T? other)
    {
        return other is null ? -1 : CompareTo(ref other);
    }
}
namespace AlirezaMahDev.Extensions.Abstractions;

public readonly ref struct ScopedReadOnlyComparableComparer<T, TComparer>(T value, TComparer comparer)
    : IScopedRefReadOnlyComparable<T>
    where TComparer : IComparer<T>, allows ref struct
{
    private readonly T _value = value;
    private readonly TComparer _comparer = comparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(scoped ref readonly T other)
    {
        return other is null ? 1 : _comparer.Compare(_value, other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(T? other)
    {
        return other is null ? -1 : CompareTo(ref other);
    }
}
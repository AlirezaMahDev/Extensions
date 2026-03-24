namespace AlirezaMahDev.Extensions.Abstractions;

public readonly ref struct ComparableComparerBridge<T, TBridge, TComparer>(
    in TBridge value,
    Func<T, TBridge> func,
    in TComparer comparer)
    : IComparable<T>
    where TComparer : IComparer<TBridge>
    where TBridge : allows ref struct
{
    private readonly TBridge _value = value;
    private readonly ref readonly TComparer _comparer = ref comparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(T? other)
    {
        return other is null ? 1 : _comparer.Compare(_value, func(other));
    }
}
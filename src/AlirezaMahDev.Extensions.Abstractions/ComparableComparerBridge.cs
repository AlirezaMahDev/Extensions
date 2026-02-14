namespace AlirezaMahDev.Extensions.Abstractions;

public readonly ref struct ComparableComparerBridge<T, TBridge, TComparer>(
    TBridge value,
    Func<T, TBridge> func,
    TComparer comparer) : IComparable<T>
    where TComparer : IComparer<TBridge>
    where TBridge : allows ref struct
{
    private readonly TBridge _value = value;
    public int CompareTo(T? other) => other is null ? 1 : comparer.Compare(_value, func(other));
}
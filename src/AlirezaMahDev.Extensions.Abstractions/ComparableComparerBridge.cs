namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ComparableComparerBridge<T, TBridge, TComparer>(
    TBridge Value,
    Func<T, TBridge> Func,
    TComparer Comparer) : IComparable<T>
    where TComparer : IComparer<TBridge>
{
    public int CompareTo(T? other) => other is null ? 1 : Comparer.Compare(Value, Func(other));
}
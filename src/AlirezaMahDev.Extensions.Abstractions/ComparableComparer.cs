namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ComparableComparison<T>(T Value, Comparison<T> Comparison) : IComparable<T>
{
    public int CompareTo(T? other) => other is null ? 1 : Comparison(Value, other);
}

public readonly record struct ComparableComparer<T, TComparer>(T Value, TComparer Comparer) : IComparable<T>
    where TComparer : IComparer<T>
{
    public int CompareTo(T? other) => other is null ? 1 : Comparer.Compare(Value, other);
}

public readonly record struct ComparableComparisonBridge<T, TBridge>(
    TBridge Value,
    Func<T, TBridge> Func,
    Comparison<TBridge> Comparison) : IComparable<T>
{
    public int CompareTo(T? other) => other is null ? 1 : Comparison(Value, Func(other));
}

public readonly record struct ComparableComparerBridge<T, TBridge, TComparer>(
    TBridge Value,
    Func<T, TBridge> Func,
    TComparer Comparer) : IComparable<T>
    where TComparer : IComparer<TBridge>
{
    public int CompareTo(T? other) => other is null ? 1 : Comparer.Compare(Value, Func(other));
}
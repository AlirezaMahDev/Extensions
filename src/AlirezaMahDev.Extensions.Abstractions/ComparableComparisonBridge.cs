namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ComparableComparisonBridge<T, TBridge>(
    TBridge Value,
    Func<T, TBridge> Func,
    Comparison<TBridge> Comparison) : IComparable<T>
{
    public int CompareTo(T? other) => other is null ? 1 : Comparison(Value, Func(other));
}
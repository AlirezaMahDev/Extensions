namespace AlirezaMahDev.Extensions.Abstractions;

public readonly ref struct ComparableComparisonBridge<T, TBridge>(
    TBridge value,
    Func<T, TBridge> func,
    Comparison<TBridge> comparison) : IComparable<T>
    where TBridge : allows ref struct
{
    private readonly TBridge _value = value;
    public int CompareTo(T? other) => other is null ? 1 : comparison(_value, func(other));
}
namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ComparableComparison<T>(T Value, Comparison<T> Comparison) : IComparable<T>
{
    public int CompareTo(T? other) => other is null ? 1 : Comparison(Value, other);
}
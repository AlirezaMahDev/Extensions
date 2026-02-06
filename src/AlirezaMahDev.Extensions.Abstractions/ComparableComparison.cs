namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ComparableComparison<T>(T Value, Comparison<T> Comparison) : IComparable<T>
{
    public int CompareTo(T? other) =>
        Comparer<T>.NullDown(Value, other) ?? Comparison(Value!, other!);
}
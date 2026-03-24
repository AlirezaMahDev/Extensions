namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ComparableComparison<T>(T Value, Comparison<T> Comparison) : IComparable<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(T? other)
    {
        return other is null ? 1 : Comparison(Value, other);
    }
}
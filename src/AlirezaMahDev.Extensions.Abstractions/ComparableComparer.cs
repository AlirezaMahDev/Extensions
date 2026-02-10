namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ComparableComparer<T, TComparer>(T Value, TComparer Comparer) : IComparable<T>
    where TComparer : IComparer<T>
{
    public int CompareTo(T? other) => other is null ? 1 : Comparer.Compare(Value, other);
}
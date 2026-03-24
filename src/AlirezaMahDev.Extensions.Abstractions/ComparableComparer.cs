namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ComparableComparer<T, TComparer>(T Value, TComparer Comparer) : IInComparable<T>
    where TComparer : IComparer<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(in T? other)
    {
        return other is null ? 1 : Comparer.Compare(Value, other);
    }
}
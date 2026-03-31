namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ComparisonCollection<T>(IEnumerable<ScopedRefReadOnlyComparison<T>> Enumerable)
    : IComparisonCollection<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ComparisonCollection<T> Create()
    {
        return new([]);
    }
}
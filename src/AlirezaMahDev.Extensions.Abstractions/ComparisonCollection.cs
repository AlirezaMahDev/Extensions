namespace AlirezaMahDev.Extensions.Abstractions;

public record struct ComparisonCollection<T>(IEnumerable<Comparison<T>> Enumerable) : IComparisonCollection<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ComparisonCollection<T> Create()
    {
        return new([]);
    }
}
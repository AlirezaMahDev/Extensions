namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ScopedComparisonChain<T>(
    ScopedRefReadOnlyComparison<T> Comparison,
    ScopedRefReadOnlyComparison<T> CurrentComparison,
    IComparisonChain<T>? PreviousComparisonChain)
    : IComparisonChain<T>, IScopedRefReadOnlyComparer<T>
    where T : allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Compare(scoped ref readonly T x, scoped ref readonly T y)
    {
        return Comparison(in x, in y);
    }
}
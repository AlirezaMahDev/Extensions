namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct ScopedRefReadOnlyComparisonToScopedRefReadOnlyComparer<T>(
    ScopedRefReadOnlyComparison<T> readOnlyComparison)
    : IScopedRefReadOnlyComparer<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Compare(scoped ref readonly T x, scoped ref readonly T y)
    {
        return readOnlyComparison(in x, in y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Compare(T? x, T? y)
    {
        return ComparerHelper<T>.NullDown(ref x, ref y) ?? readOnlyComparison(ref x!, ref y!);
    }
}
namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ScopedComparisonBridge<T, TBridge>(
    ScopedRefReadOnlyFunc<T, TBridge> Func,
    ScopedRefReadOnlyComparison<TBridge> Comparison) : IScopedRefReadOnlyComparer<T>
    where TBridge : allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Compare(scoped ref readonly T x, scoped ref readonly T y)
    {
        var bridgeX = Func(in x);
        var bridgeY = Func(in y);
        return Comparer<T>.NullDown(x, y) ?? Comparison(ref bridgeX, ref bridgeY);
    }
}
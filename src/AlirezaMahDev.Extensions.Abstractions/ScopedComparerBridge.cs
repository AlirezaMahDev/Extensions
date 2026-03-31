namespace AlirezaMahDev.Extensions.Abstractions;

public readonly record struct ScopedComparerBridge<T, TBridge, TComparer>(
    ScopedRefReadOnlyFunc<T, TBridge> Func,
    TComparer Comparer) : IScopedRefReadOnlyComparer<T>
    where TComparer : IScopedRefReadOnlyComparer<TBridge>
    where TBridge : allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Compare(scoped ref readonly T x, scoped ref readonly T y)
    {
        var bridgeX = Func(in x);
        var bridgeY = Func(in y);
        return Comparer.Compare(in bridgeX, in bridgeY);
    }
}
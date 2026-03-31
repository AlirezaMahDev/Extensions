namespace AlirezaMahDev.Extensions.Abstractions;

public interface IScopedRefReadOnlyComparer<T> : IComparer<T>
    where T : allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    int Compare(scoped ref readonly T x, scoped ref readonly T y);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    int IComparer<T>.Compare(T? x, T? y)
    {
        return ComparerHelper<T>.NullDown(ref x, ref y) ?? Compare(ref x!, ref y!);
    }
}
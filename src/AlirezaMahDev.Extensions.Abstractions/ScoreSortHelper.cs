namespace AlirezaMahDev.Extensions.Abstractions;

public static class ScoreSortHelper<T>
    where T : IScoreSortItem
{
    public static ScopedRefReadOnlyComparison<T> ReadOnlyComparison { get; } =
        static (scoped ref readonly x, scoped ref readonly y) => x.Score.CompareTo(y.Score);
}
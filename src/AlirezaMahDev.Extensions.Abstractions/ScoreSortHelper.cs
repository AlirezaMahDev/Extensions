namespace AlirezaMahDev.Extensions.Abstractions;

public static class ScoreSortHelper<T>
    where T : IScoreSortItem
{
    public static Comparison<T> Comparison { get; } = static (x, y) => x.Score.CompareTo(y.Score);
}
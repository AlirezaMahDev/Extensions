namespace AlirezaMahDev.Extensions.Abstractions;

public readonly struct ScoreSortItemComparer<T> : IComparer<T>
    where T : IScoreSortItem
{
    public static ScoreSortItemComparer<T> Default { get; } = new();

    public int Compare(T? x, T? y) =>
        x is null
            ? 1
            : y is null
                ? 1
                : x.Score.CompareTo(y.Score);
}
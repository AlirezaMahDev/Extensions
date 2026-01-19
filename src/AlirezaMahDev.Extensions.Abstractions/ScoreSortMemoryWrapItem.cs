namespace AlirezaMahDev.Extensions.Abstractions;

public record ScoreSortMemoryWrapItem<T>(T Value) : IScoreSortItem
    where T : notnull
{
    public int Score { get; set; }
}
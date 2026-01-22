namespace AlirezaMahDev.Extensions.Abstractions;

public record ScoreSortItem<T>(T Value) : IScoreSortItem
    where T : notnull
{
    public int Score { get; set; }
}
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public record struct ScoreSortItem<T>(T Value) : IScoreSortItem
    where T : notnull
{
    public int Score { get; set; }
}
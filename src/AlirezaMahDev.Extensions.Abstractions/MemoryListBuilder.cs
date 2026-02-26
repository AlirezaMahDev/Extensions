namespace AlirezaMahDev.Extensions.Abstractions;

public static class MemoryListBuilder
{
    public static MemoryList<T> Create<T>(ReadOnlySpan<T> values) => new(values);
}
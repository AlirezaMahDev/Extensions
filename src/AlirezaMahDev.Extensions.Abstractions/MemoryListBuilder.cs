namespace AlirezaMahDev.Extensions.Abstractions;

public static class MemoryListBuilder
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    [MustDisposeResource]
    public static MemoryList<T> Create<T>(ReadOnlySpan<T> values)
    {
        return new(values);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    [MustDisposeResource]
    public static MemoryList<T> Create<T>(int capacity, ReadOnlySpan<T> values)
    {
        return new(capacity, values);
    }
}
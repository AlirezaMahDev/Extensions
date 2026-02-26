using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class MemoryListExtensions
{
    extension<T>(IEnumerable<T> values)
    {
        [MustDisposeResource]
        public MemoryList<T> ToMemoryList() => [.. values];
    }

    extension<T>(IReadOnlyCollection<T> values)
    {
        [MustDisposeResource]
        public MemoryList<T> ToMemoryList() => [.. values];
    }

    extension<T>(ICollection<T> values)
    {
        [MustDisposeResource]
        public MemoryList<T> ToMemoryList() => [.. values];
    }

    extension<T>(ReadOnlySpan<T> values)
    {
        [MustDisposeResource]
        public MemoryList<T> ToMemoryList() => values;
    }

    extension<T>(Span<T> values)
    {
        [MustDisposeResource]
        public MemoryList<T> ToMemoryList() => values;
    }

    extension<T>(ReadOnlyMemory<T> values)
    {
        [MustDisposeResource]
        public MemoryList<T> ToMemoryList() => values;
    }

    extension<T>(Memory<T> values)
    {
        [MustDisposeResource]
        public MemoryList<T> ToMemoryList() => values;
    }
}
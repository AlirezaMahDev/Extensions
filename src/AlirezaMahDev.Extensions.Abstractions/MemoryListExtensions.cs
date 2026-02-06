using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class MemoryListExtensions
{
    extension<T>(IEnumerable<T> enumerable)
    {
        [MustDisposeResource]
        public MemoryList<T> ToMemoryList() =>
            [.. enumerable];
    }
    extension<T>(IReadOnlyCollection<T> enumerable)
    {
        [MustDisposeResource]
        public MemoryList<T> ToMemoryList() =>
            new(enumerable.Count, enumerable);
    }
    extension<T>(ICollection<T> enumerable)
    {
        [MustDisposeResource]
        public MemoryList<T> ToMemoryList() =>
            new(enumerable.Count, enumerable);
    }
}

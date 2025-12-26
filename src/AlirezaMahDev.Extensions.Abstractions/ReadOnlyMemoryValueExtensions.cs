namespace AlirezaMahDev.Extensions.Abstractions;

public static class ReadOnlyMemoryValueExtensions
{
    extension<T>(ReadOnlyMemory<T> readOnlyMemory)
    {
        public ReadOnlyMemoryValue<T> ElementAt(int index) =>
            new(readOnlyMemory.Slice(index, 1));
    }
}
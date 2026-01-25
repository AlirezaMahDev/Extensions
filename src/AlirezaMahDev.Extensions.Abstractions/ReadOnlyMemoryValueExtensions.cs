namespace AlirezaMahDev.Extensions.Abstractions;

public static class ReadOnlyMemoryValueExtensions
{
    extension<T>(T t)
    {
        public ReadOnlyMemoryValue<T> AsReadonlyMemoryValue() =>
            new(t);
    }

    extension<T>(ReadOnlyMemory<T> readOnlyMemory)
    {
        public ReadOnlyMemoryValue<T> ElementAt(int index) =>
            new(readOnlyMemory.Slice(index, 1));
    }
}
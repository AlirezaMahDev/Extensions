namespace AlirezaMahDev.Extensions.Abstractions;

public static class ReadOnlyMemoryValueExtensions
{
    extension<T>(ref T t)
        where T : struct
    {
        public ReadOnlyMemoryValue<T> AsReadonlyMemoryValue() =>
            new(in t);
    }

    extension<T>(ReadOnlyMemory<T> readOnlyMemory)
        where T : struct
    {
        public ReadOnlyMemoryValue<T> ElementAt(int index) => new(readOnlyMemory.Slice(index, 1));
    }

    extension<T>(ReadOnlyMemoryValue<T> readOnlyMemoryValue)
        where T : struct
    {
        public ReadOnlySpanValue<T> AsReadOnlySpanValue() => new(in readOnlyMemoryValue.Value);
    }
}
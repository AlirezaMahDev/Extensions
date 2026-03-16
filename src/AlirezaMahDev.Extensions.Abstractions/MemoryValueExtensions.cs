namespace AlirezaMahDev.Extensions.Abstractions;

public static class MemoryValueExtensions
{
    extension<T>(ref T t)
        where T : struct
    {
        public MemoryValue<T> AsMemoryValue()
        {
            return new(in t);
        }
    }

    extension<T>(Memory<T> memory)
        where T : struct
    {
        public MemoryValue<T> ElementAt(int index)
        {
            return new(memory.Slice(index, 1));
        }
    }

    extension<T>(MemoryValue<T> memoryValue)
        where T : struct
    {
        public ReadOnlyMemoryValue<T> AsReadOnlyMemoryValue()
        {
            return memoryValue;
        }

        public SpanValue<T> AsSpanValue()
        {
            return new(ref memoryValue.Value);
        }

        public ReadOnlySpanValue<T> AsReadOnlySpanValue()
        {
            return new(in memoryValue.Value);
        }
    }
}
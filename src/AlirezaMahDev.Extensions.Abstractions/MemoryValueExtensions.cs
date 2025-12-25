namespace AlirezaMahDev.Extensions.Abstractions;

public static class MemoryValueExtensions
{
    extension<T>(MemoryValue<T> memoryValue)
    {
        public ReadOnlyMemory<T> AsReadOnlyMemory() => memoryValue.Memory;
    }

    extension<T>(Memory<T> memory)
    {
        public MemoryValue<T> ElementAt(int index) =>
            new(memory.Slice(index, 1));
    }
}

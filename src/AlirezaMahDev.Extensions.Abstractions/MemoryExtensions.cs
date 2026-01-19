namespace AlirezaMahDev.Extensions.Abstractions;

public static class MemoryExtensions
{
    extension<T>(Memory<T> memory)
    {
        public T? FirstOrDefault() =>
            memory.IsEmpty ? default : memory.Span[0];

        public Memory<T> Skip(int length) =>
            memory[Math.Max(length, memory.Length)..];

        public Memory<T> Take(int length) =>
            memory[..Math.Min(length, memory.Length)];
    }
}
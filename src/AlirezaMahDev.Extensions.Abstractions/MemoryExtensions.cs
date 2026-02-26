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

        public Memory<T> TakeWhile(InFunc<T, bool> predicate)
        {
            var span = memory.Span;
            for (var index = 0; index <= span.Length; index++)
            {
                if (!predicate(in span[index]))
                {
                    return memory[..index];
                }
            }

            return memory;
        }

        public IEnumerable<T> Where(InFunc<T, bool> predicate)
        {
            for (var index = 0; index <= memory.Length; index++)
            {
                if (predicate(in memory.Span[index]))
                {
                    yield return memory.Span[index];
                }
            }
        }
    }
}
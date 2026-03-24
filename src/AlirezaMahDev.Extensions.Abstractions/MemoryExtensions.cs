namespace AlirezaMahDev.Extensions.Abstractions;

public static class MemoryExtensions
{
    extension<T>(Memory<T> memory)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public T? FirstOrDefault()
        {
            return memory.IsEmpty ? default : memory.Span[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Memory<T> Skip(int length)
        {
            return memory[Math.Max(length, memory.Length)..];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Memory<T> Take(int length)
        {
            return memory[..Math.Min(length, memory.Length)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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
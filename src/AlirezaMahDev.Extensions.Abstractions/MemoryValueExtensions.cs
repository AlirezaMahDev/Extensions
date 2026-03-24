namespace AlirezaMahDev.Extensions.Abstractions;

public static class MemoryValueExtensions
{
    extension<T>(ref T t)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public MemoryValue<T> AsMemoryValue()
        {
            return new(in t);
        }
    }

    extension<T>(Memory<T> memory)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public MemoryValue<T> ElementAt(int index)
        {
            return new(memory.Slice(index, 1));
        }
    }

    extension<T>(MemoryValue<T> memoryValue)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlyMemoryValue<T> AsReadOnlyMemoryValue()
        {
            return memoryValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public SpanValue<T> AsSpanValue()
        {
            return new(ref memoryValue.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpanValue<T> AsReadOnlySpanValue()
        {
            return new(in memoryValue.Value);
        }
    }
}
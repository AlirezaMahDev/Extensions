namespace AlirezaMahDev.Extensions.Abstractions;

public static class ReadOnlyMemoryValueExtensions
{
    extension<T>(ref T t)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlyMemoryValue<T> AsReadonlyMemoryValue()
        {
            return new(in t);
        }
    }

    extension<T>(ReadOnlyMemory<T> readOnlyMemory)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlyMemoryValue<T> ElementAt(int index)
        {
            return new(readOnlyMemory.Slice(index, 1));
        }
    }

    extension<T>(ReadOnlyMemoryValue<T> readOnlyMemoryValue)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpanValue<T> AsReadOnlySpanValue()
        {
            return new(in readOnlyMemoryValue.Value);
        }
    }
}
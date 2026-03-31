namespace AlirezaMahDev.Extensions.Abstractions;

public static class ReadOnlySpanValueExtensions
{
    extension<T>(ref T t)
        where T : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ReadOnlySpanValue<T> AsReadOnlySpanValue()
        {
            return new(ref t);
        }
    }
}
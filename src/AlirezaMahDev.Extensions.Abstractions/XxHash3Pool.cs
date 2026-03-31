using System.Collections.Concurrent;
using System.IO.Hashing;

namespace AlirezaMahDev.Extensions.Abstractions;

internal static class XxHash3Pool
{
    private static readonly ConcurrentBag<XxHash3> Pool = [];
    private const int MaxPoolSize = 64;
    private static int s_count;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static XxHash3 Rent()
    {
        if (Pool.TryTake(out var hasher))
        {
            Interlocked.Decrement(ref s_count);
            hasher.Reset();
            return hasher;
        }

        return new();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Return(XxHash3 hasher)
    {
        if (Interlocked.Increment(ref s_count) <= MaxPoolSize)
        {
            Pool.Add(hasher);
        }
        else
        {
            Interlocked.Decrement(ref s_count);
        }
    }
}
using System.IO.Hashing;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class XxHash3Extensions
{
    extension(XxHash3)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int Combine<T1>(in T1 value1)
            where T1 : unmanaged
        {
            using var builder = XxHash3.Builder();
            builder.Add(in value1);
            return builder.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int Combine<T1, T2>(in T1 value1, in T2 value2)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            using var builder = XxHash3.Builder();
            builder.Add(in value1);
            builder.Add(in value2);
            return builder.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int Combine<T1, T2, T3>(
            in T1 value1,
            in T2 value2,
            in T3 value3)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
        {
            using var builder = XxHash3.Builder();
            builder.Add(in value1);
            builder.Add(in value2);
            builder.Add(in value3);
            return builder.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int Combine<T1, T2, T3, T4>(
            in T1 value1,
            in T2 value2,
            in T3 value3,
            in T4 value4)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
        {
            using var builder = XxHash3.Builder();
            builder.Add(in value1);
            builder.Add(in value2);
            builder.Add(in value3);
            builder.Add(in value4);
            return builder.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int Combine<T1, T2, T3, T4, T5>(
            in T1 value1,
            in T2 value2,
            in T3 value3,
            in T4 value4,
            in T5 value5)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
            where T5 : unmanaged
        {
            using var builder = XxHash3.Builder();
            builder.Add(in value1);
            builder.Add(in value2);
            builder.Add(in value3);
            builder.Add(in value4);
            builder.Add(in value5);
            return builder.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int CombineSpan<T>(ReadOnlySpan<T> values)
            where T : unmanaged
        {
            using var builder = XxHash3.Builder();
            builder.AddSpan(values);
            return builder.ToHashCode();
        }

        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static XxHash3Builder Builder()
        {
            return new();
        }
    }
}
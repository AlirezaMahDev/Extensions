using System.Diagnostics.CodeAnalysis;
using System.IO.Hashing;
using System.Runtime.Intrinsics;

namespace AlirezaMahDev.Extensions.Abstractions;

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct ConcurrencyIndex(int shardingIndex, int shardingItemIndex) : IScopedRefReadOnlyEquatable<ConcurrencyIndex>, IScopedInEqualityOperators<ConcurrencyIndex, ConcurrencyIndex, bool>
{
    public static readonly ConcurrencyIndex Null = new(-1, -1);
    public readonly int ShardingIndex = shardingIndex;
    public readonly int ShardingItemIndex = shardingItemIndex;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static ref ConcurrencyIndex FromRefLong(ref long value) =>
        ref Unsafe.As<long, ConcurrencyIndex>(ref value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly ref long ToRefLong()
    {
        return ref Unsafe.As<ConcurrencyIndex, long>(ref Unsafe.AsRef(in this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(scoped ref readonly ConcurrencyIndex other)
    {
        return ToRefLong() == other.ToRefLong();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(scoped in ConcurrencyIndex left, scoped in ConcurrencyIndex right)
    {
        return left.Equals(in right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(scoped in ConcurrencyIndex left, scoped in ConcurrencyIndex right)
    {
        return !left.Equals(in right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ConcurrencyIndex concurrencyIndex && Equals(in concurrencyIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return XxHash3.Combine(in ShardingIndex, in ShardingItemIndex);
    }
}
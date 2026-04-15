namespace AlirezaMahDev.Extensions.Abstractions;

public static class NativeConcurrencyRefBagCollectionBuilder
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeConcurrencyRefBag<T> Create<T>(ReadOnlySpan<T> readOnlySpan)
        where T : unmanaged => NativeConcurrencyRefBag<T>.Create(readOnlySpan);
}
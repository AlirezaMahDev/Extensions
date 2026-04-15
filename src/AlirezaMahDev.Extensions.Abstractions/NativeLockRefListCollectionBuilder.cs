namespace AlirezaMahDev.Extensions.Abstractions;

public static class NativeLockRefListCollectionBuilder
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeLockRefList<T> Create<T>(ReadOnlySpan<T> readOnlySpan)
        where T : unmanaged => NativeLockRefList<T>.Create(readOnlySpan);
}
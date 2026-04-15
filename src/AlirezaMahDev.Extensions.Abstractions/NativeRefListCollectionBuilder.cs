namespace AlirezaMahDev.Extensions.Abstractions;

public static class NativeRefListCollectionBuilder
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeRefList<T> Create<T>(ReadOnlySpan<T> readOnlySpan)
        where T : unmanaged => NativeRefList<T>.Create(readOnlySpan);
}
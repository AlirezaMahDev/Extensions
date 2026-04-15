namespace AlirezaMahDev.Extensions.Abstractions;

public static class RefReadOnlyIndexableExtensions
{
    extension<TSelf, T>(TSelf self)
        where TSelf : IRefReadOnlyIndexable<TSelf, T>, allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public RefReadOnlyBlock<TSelf, T> AsRefReadOnlyBlock()
        {
            return new(self);
        }
    }
}
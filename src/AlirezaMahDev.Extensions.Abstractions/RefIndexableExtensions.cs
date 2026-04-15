namespace AlirezaMahDev.Extensions.Abstractions;

public static class RefIndexableExtensions
{
    extension<TSelf, T>(TSelf self)
        where TSelf : IRefIndexable<TSelf, T>, allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public RefBlock<TSelf, T> AsRefBlock()
        {
            return new(self);
        }
    }
}
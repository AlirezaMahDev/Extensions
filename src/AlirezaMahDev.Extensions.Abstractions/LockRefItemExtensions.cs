namespace AlirezaMahDev.Extensions.Abstractions;

public static class LockRefItemExtensions
{
    extension<T>(LockRefItem<T> self)
    {
        public T CopyValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                using var item = self;
                return item.Value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult GetCopyValue<TResult>(ScopedRefFunc<T, TResult> scopedRefFunc)
        {
            using var item = self;
            return scopedRefFunc(ref item.Value);
        }
    }
}
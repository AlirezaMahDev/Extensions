namespace AlirezaMahDev.Extensions.Abstractions;

public static class ConcurrencyRefPoolExtensions
{
    extension<TSelf, T>(TSelf self)
        where TSelf : IConcurrencyRefPool<TSelf, T>, allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ConcurrencyIndex Rent(T value)
        {
            var item = self.Rent();
            using LockRefItem<T> itemLocker = item.Value;
            itemLocker.Value = value;
            return item.Index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Return(in ConcurrencyRefIndexableItem<TSelf, T> item)
        {
            self.Return(item.Index);
        }
    }
}
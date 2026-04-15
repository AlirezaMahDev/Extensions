namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefPool<TSelf, T> : ILockRefIndexable<TSelf, T>
    where TSelf : ILockRefPool<TSelf, T>, allows ref struct
{
    bool? TryRent(out LockRefIndexableItem<TSelf, T> item, int timeout = -1);
    bool? TryReturn(int index, int timeout = -1);
    void Clean();
}
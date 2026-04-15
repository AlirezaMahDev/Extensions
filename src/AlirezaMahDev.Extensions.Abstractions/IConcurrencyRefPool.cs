namespace AlirezaMahDev.Extensions.Abstractions;

public interface IConcurrencyRefPool<TSelf, T> : IConcurrencyRefIndexable<TSelf, T>
    where TSelf : IConcurrencyRefPool<TSelf, T>, allows ref struct
{
    ConcurrencyRefIndexableItem<TSelf, T> Rent();
    void Return(ConcurrencyIndex index);
    void Clean();
}
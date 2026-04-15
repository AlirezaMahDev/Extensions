using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IConcurrencyRefBag<TSelf, T> : IConcurrencyRefIndexable<TSelf, T>
    where TSelf : IConcurrencyRefBag<TSelf, T>, allows ref struct
{
    ConcurrencyIndex TryAdd(in T value);
    bool TryGet(in ConcurrencyIndex index, out LockRefItem<T> result);
    bool TryGet(out LockRefItem<T> result);
    bool TryAdd(Span<ConcurrencyIndex> indices, params ReadOnlySpan<T> values);
    bool TryRemove(in ConcurrencyIndex index, [NotNullWhen(true)] out T? result);
    bool TryRemove([NotNullWhen(true)] out T? result);
    void Clean();
}
using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IConcurrencyRefStack<TSelf, T> : IConcurrencyRefIndexable<TSelf, T>
    where TSelf : IConcurrencyRefStack<TSelf, T>, allows ref struct
{
    bool TryPop([NotNullWhen(true)] out T? result);
    bool TryPeek(out ConcurrencyRefIndexableItem<TSelf, T> result);
    bool TryPush(in T value);
    void Clean();
}
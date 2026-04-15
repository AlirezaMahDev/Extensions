using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IConcurrencyRefQueue<TSelf, T> : IConcurrencyRefIndexable<TSelf, T>
    where TSelf : IConcurrencyRefQueue<TSelf, T>, allows ref struct
{
    bool TryPeek(out ConcurrencyRefIndexableItem<TSelf, T> result);
    bool TryDequeue([NotNullWhen(true)] out T? result);
    bool TryEnqueue(in T value);
    void Clean();
}
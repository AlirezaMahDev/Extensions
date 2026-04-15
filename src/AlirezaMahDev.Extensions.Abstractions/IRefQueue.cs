using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefQueue<TSelf, T> : IRefIndexable<TSelf, T>
    where TSelf : IRefQueue<TSelf, T>, allows ref struct
{
    bool TryPeek(out RefIndexableItem<TSelf, T> result);
    bool TryDequeue([NotNullWhen(true)] out T? result);
    bool TryEnqueue(in T value);
    void Clean();
}
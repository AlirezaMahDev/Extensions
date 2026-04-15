using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefStack<TSelf, T> : IRefIndexable<TSelf, T>
    where TSelf : IRefStack<TSelf, T>, allows ref struct
{
    bool TryPop([NotNullWhen(true)] out T? result);
    bool TryPeek(out RefIndexableItem<TSelf, T> result);
    bool TryPush(in T value);
    void Clean();
}
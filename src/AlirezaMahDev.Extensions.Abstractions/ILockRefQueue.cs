using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefQueue<TSelf, T> : ILockRefIndexable<TSelf, T>
    where TSelf : ILockRefQueue<TSelf, T>, allows ref struct
{
    bool? TryPeek(out LockRefIndexableItem<TSelf, T> result, int timeout = -1);
    bool? TryDequeue([NotNullWhen(true)] out T? result, int timeout = -1);
    bool? TryEnqueue(in T value, int timeout = -1);
    void Clean();
}
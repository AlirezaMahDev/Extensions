using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefStack<TSelf, T> : ILockRefIndexable<TSelf, T>
    where TSelf : ILockRefStack<TSelf, T>, allows ref struct
{
    bool? TryPop([NotNullWhen(true)] out T? result, int timeout = -1);
    bool? TryPeek(out LockRefIndexableItem<TSelf, T> result, int timeout = -1);
    bool? TryPush(in T value, int timeout = -1);
    void Clean();
}
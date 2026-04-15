using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockRefList<TSelf, T> : ILockRefIndexable<TSelf, T>
    where TSelf : ILockRefList<TSelf, T>, allows ref struct
{
    bool? TryGet(int index, out LockRefItem<T> result, int timeout = -1);
    int? TryAdd(in T value, int timeout = -1);
    int? TryAdd(ReadOnlySpan<T> values, int timeout = -1);
    bool? TryInsert(int index, in T value, int timeout = -1);
    bool? TryInsert(int index, ReadOnlySpan<T> values, int timeout = -1);
    bool? TryRemove(int index, [NotNullWhen(true)] out T? result, int timeout = -1);
    bool? TryRemove(int index, Span<T> result, int timeout = -1);
    void Clean();
}
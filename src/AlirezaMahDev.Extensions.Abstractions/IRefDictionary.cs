using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IRefDictionary<TSelf, TKey, TValue> : IRefLength
    where TSelf : IRefDictionary<TSelf, TKey, TValue>, allows ref struct
{
    bool TryGet(in TKey key, out TValue value);
    bool TryRemove(in TKey key, [NotNullWhen(true)] out TValue value);
    bool TryAdd(in TKey key, in TValue value);
    bool TryAdd(in TKey key, ScopedRefReadOnlyFunc<TKey, TValue> func);
    TValue GetOrAdd(in TKey key, in TValue value);
    TValue GetOrAdd(in TKey key, ScopedRefReadOnlyFunc<TKey, TValue> func);
    void Clean();
}
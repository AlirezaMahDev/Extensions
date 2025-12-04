using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public interface IDataDictionary<in TKey, TItem> : IEnumerable<TItem>
{
    bool TryGet(TKey key, [MaybeNullWhen(false)] out TItem value);
    TItem GetOrAdd(TKey key);
    bool TryRemove(TKey key, [MaybeNullWhen(false)] out TItem item);
}

public interface IDataDictionary<in TKey, in TValue, TItem> : IEnumerable<TItem>
{
    bool TryGet(TKey key, [MaybeNullWhen(false)] out TItem value);
    TItem GetOrAdd(TKey key, TValue size);
    bool TryRemove(TKey key, [MaybeNullWhen(false)] out TItem item);
}
namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class DataDictionaryExtensions
{
    public static TItem GetOrAdd<TKey, TItem>(this IDataDictionary<TKey, TItem> items,
        TKey key,
        Action<TItem> action)
    {
        var item = items.GetOrAdd(key);
        action(item);
        return item;
    }

    public static bool TryRemove<TKey, TItem>(this IDataDictionary<TKey, TItem> items, TItem item)
        where TItem : IDataDictionaryItem<TKey>
    {
        return items.TryRemove(item.Key, out _);
    }
}
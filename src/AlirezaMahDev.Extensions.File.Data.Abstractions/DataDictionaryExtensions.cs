namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class DataDictionaryExtensions
{
    extension<TKey, TItem>(IDataDictionary<TKey, TItem> items)
    {
        public TItem GetOrAdd(TKey key,
            Action<TItem> action)
        {
            var item = items.GetOrAdd(key);
            action(item);
            return item;
        }
    }

    extension<TKey, TItem>(IDataDictionary<TKey, TItem> items)
        where TItem : IDataDictionaryItem<TKey>
    {
        public bool TryRemove(TItem item)
        {
            return items.TryRemove(item.Key, out _);
        }
    }
}
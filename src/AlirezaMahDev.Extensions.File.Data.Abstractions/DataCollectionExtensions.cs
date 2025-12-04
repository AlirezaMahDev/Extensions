namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class DataCollectionExtensions
{
    public static TItem Add<TItem>(this IDataCollection<TItem> items, Action<TItem> action)
    {
        var item = items.Add();
        action(item);
        return item;
    }

    public static TItem Add<TValue, TItem>(this IDataCollection<TValue, TItem> items,
        TValue value,
        Action<TItem> action)
    {
        var item = items.Add(value);
        action(item);
        return item;
    }
}
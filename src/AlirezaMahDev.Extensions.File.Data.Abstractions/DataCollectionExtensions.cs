namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class DataCollectionExtensions
{
    extension<TItem>(IDataCollection<TItem> items)
    {
        public TItem Add(Action<TItem> action)
        {
            var item = items.Add();
            action(item);
            return item;
        }
    }

    extension<TValue, TItem>(IDataCollection<TValue, TItem> items)
    {
        public TItem Add(TValue value,
        Action<TItem> action)
        {
            var item = items.Add(value);
            action(item);
            return item;
        }
    }
}
namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public interface IDataDictionaryItem<out TKey>
{
    TKey Key { get; }
    void Remove();
}

public interface IDataDictionaryItem<out TKey, out TValue> : IDataDictionaryItem<TKey>
{
    TValue Value { get; }
}
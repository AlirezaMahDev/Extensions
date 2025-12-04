namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public interface IDataCollectionItem : IDataCollectionItemBase
{
    int Index { get; }
    int Capacity { get; }
    int SubIndex { get; }
}

public interface IDataCollectionItem<out TValue> : IDataCollectionItem
{
    TValue Value { get; }
}
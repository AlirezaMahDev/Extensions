namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public interface IDataLocationItem : IDataBlockAccessorSave
{
    IDataLocation Location { get; }
    ref String64 RefKey { get; }
}

public interface IDataLocationItem<TValue> : IDataLocationItem
    where TValue : unmanaged
{
    IDataLocation IDataLocationItem.Location => LocationValue.Location;
    DataLocation<TValue> LocationValue { get; }
    ref TValue RefValue { get; }
}
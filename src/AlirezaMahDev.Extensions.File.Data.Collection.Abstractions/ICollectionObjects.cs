using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public interface ICollectionObjects : IDataLocationItem<CollectionObjectsValue>, IDataCollection<ICollectionObject>
{
    ICollectionAccess Access { get; }
}
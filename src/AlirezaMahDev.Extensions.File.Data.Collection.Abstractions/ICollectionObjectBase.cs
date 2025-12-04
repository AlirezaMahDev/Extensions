using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public interface ICollectionObjectBase : IDataLocationItem<CollectionObjectValue>, IDataCollectionItemBase
{
    ICollectionProperties Properties { get; }
}
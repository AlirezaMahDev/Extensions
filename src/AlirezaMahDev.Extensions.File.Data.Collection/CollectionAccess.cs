using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

internal class CollectionAccess : ICollectionAccess
{
    public CollectionAccess(
        IDataLocation dataLocation,
        CollectionObjectsFactory collectionObjectsFactory)
    {
        Location = dataLocation.GetOrAdd(".collections");
        Objects = collectionObjectsFactory.GetOrCreate(this);
    }

    public ICollectionObjects Objects { get; }
    public IDataLocation Location { get; }

    public ref String64 RefKey => ref Location.RefKey;

    public void Save()
    {
        Location.Save();
        Objects.Save();
    }
}
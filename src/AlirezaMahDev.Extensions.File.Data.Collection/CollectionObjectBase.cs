using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

public class CollectionObjectBase : ICollectionObjectBase
{
    protected CollectionObjectBase(IDataLocation location)
    {
        LocationValue = location.GetOrAdd<CollectionObjectValue>(".object");
        Properties = new CollectionPropertiesBase(LocationValue.Location);
    }

    public CollectionObjectBase(IDataLocation location, ICollectionProperties properties)
    {
        LocationValue = location.GetOrAdd<CollectionObjectValue>(".object");
        Properties = properties;
    }
    public DataLocation<CollectionObjectValue> LocationValue { get; }
    public virtual ICollectionProperties Properties { get; }

    public ref String64 RefKey => ref LocationValue.RefKey;
    public ref CollectionObjectValue RefValue => ref LocationValue.RefValue;

    public void Save()
    {
        LocationValue.Save();
        Properties.Save();
    }

    public void Remove()
    {
        RefValue.DeleteAt = DateTimeOffset.UtcNow;
    }
}
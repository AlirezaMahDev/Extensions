using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

public class CollectionPropertyBase(IDataLocation location, String64 key) : ICollectionProperty
{
    public DataLocation<CollectionPropertyValue> LocationValue { get; } =
        location.GetOrAdd<CollectionPropertyValue>(key);

    public ref String64 RefKey => ref LocationValue.RefKey;
    public ref CollectionPropertyValue RefValue => ref LocationValue.RefValue;

    public void Save()
    {
        LocationValue.Save();
    }
}
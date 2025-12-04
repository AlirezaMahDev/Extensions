using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

internal class CollectionProperties(CollectionObject o, CollectionPropertyFactory propertyFactory)
    : CollectionPropertiesBase(o.LocationValue.Location.GetOrAdd(".properties"))
{
    protected override ICollectionProperty CreateProperty(String64 key)
    {
        return propertyFactory.GetOrCreate(new(this, key));
    }
}
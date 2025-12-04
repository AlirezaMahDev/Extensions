using Microsoft.Extensions.DependencyInjection;

using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

public static class CollectionAccessExtensions
{
    public static ICollectionAccess AsCollection(this IDataLocation location)
    {
        return location.DataAccess.FileAccess.Provider.GetRequiredService<CollectionAccessFactory>()
            .GetOrCreate(location);
    }
}
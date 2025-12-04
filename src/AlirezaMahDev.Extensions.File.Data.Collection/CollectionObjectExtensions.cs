using Microsoft.Extensions.DependencyInjection;

using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

public static class CollectionObjectExtensions
{
    public static ICollectionObjectBase AsObject(this IDataLocation location)
    {
        return ActivatorUtilities.CreateInstance<CollectionObjectBase>(location.DataAccess.FileAccess.Provider,
            location);
    }
}
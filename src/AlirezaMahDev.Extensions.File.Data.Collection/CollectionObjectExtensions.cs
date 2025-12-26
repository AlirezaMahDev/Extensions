using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

public static class CollectionObjectExtensions
{
    extension(IDataLocation location)
    {
        public ICollectionObjectBase AsObject()
        {
            return ActivatorUtilities.CreateInstance<CollectionObjectBase>(location.DataAccess.FileAccess.Provider,
                location);
        }
    }
}
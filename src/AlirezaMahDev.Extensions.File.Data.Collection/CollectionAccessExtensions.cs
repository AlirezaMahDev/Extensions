using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

public static class CollectionAccessExtensions
{
    extension(IDataLocation location)
    {
        public ICollectionAccess AsCollection()
        {
            return location.DataAccess.FileAccess.Provider.GetRequiredService<CollectionAccessFactory>()
                .GetOrCreate(location);
        }
    }
}
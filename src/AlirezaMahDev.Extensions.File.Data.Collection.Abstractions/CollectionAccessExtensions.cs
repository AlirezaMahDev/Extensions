namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public static class CollectionAccessExtensions
{
    extension(ICollectionAccess dataLocation)
    {
        public CollectionAccess<TEntity> As<TEntity>()
        where TEntity : class
        {
            return new(dataLocation);
        }
    }
}
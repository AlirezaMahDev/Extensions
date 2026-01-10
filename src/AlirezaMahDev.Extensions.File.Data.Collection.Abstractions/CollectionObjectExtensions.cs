namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public static class CollectionObjectExtensions
{
    extension(ICollectionObject collectionObject)
    {
        public CollectionObject<TEntity> As<TEntity>()
            where TEntity : class
        {
            return new(collectionObject);
        }
    }
}
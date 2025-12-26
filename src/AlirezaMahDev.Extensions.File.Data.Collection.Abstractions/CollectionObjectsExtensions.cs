namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public static class CollectionObjectsExtensions
{
    extension(ICollectionObjects collectionObjects)
    {
        public CollectionObjects<TEntity> As<TEntity>()
        where TEntity : class
        {
            return new(collectionObjects);
        }
    }
}
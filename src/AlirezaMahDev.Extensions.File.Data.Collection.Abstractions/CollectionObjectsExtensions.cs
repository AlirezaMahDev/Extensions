namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public static class CollectionObjectsExtensions
{
    public static CollectionObjects<TEntity> As<TEntity>(this ICollectionObjects collectionObjects)
        where TEntity : class
    {
        return new(collectionObjects);
    }
}
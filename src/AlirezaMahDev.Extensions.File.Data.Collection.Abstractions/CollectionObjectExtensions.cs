namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public static class CollectionObjectExtensions
{
    public static CollectionObject<TEntity> As<TEntity>(this ICollectionObject collectionObject)
        where TEntity : class
    {
        return new(collectionObject);
    }
}
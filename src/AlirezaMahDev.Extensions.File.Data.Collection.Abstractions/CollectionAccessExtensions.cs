namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public static class CollectionAccessExtensions
{
    public static CollectionAccess<TEntity> As<TEntity>(this ICollectionAccess dataLocation)
        where TEntity : class
    {
        return new(dataLocation);
    }
}
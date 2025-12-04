using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public readonly record struct CollectionObject<TEntity>(ICollectionObject Object)
    where TEntity : class
{
    public ref String64 RefKey => ref Object.RefKey;
    public TEntity Entity { get; } = Object.LocationValue.Location.AsObject<TEntity>().Entity;
}
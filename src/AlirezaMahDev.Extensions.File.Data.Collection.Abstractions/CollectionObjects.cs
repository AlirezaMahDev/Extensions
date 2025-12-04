using System.Collections;

using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public readonly record struct CollectionObjects<TEntity>(ICollectionObjects Objects)
    : IDataCollection<CollectionObject<TEntity>>
    where TEntity : class
{
    public ICollectionObjects Objects { get; } = Objects;

    public int Count => Objects.Count;

    public CollectionObject<TEntity> Add()
    {
        return Objects.Add().As<TEntity>();
    }

    public bool TryGet(int index, out CollectionObject<TEntity> item)
    {
        if (Objects.TryGet(index, out var collectionObject))
        {
            item = collectionObject.As<TEntity>();
            return true;
        }

        item = default;
        return false;
    }

    public CollectionObject<TEntity> GetOrAdd(int index)
    {
        return Objects.GetOrAdd(index).As<TEntity>();
    }

    public bool TryRemove(int index, out CollectionObject<TEntity> @object)
    {
        if (Objects.TryRemove(index, out var collectionItem))
        {
            @object = collectionItem.As<TEntity>();
            return true;
        }

        @object = default;
        return false;
    }

    public IEnumerator<CollectionObject<TEntity>> GetEnumerator()
    {
        return Count.ToIdRange().Select(GetOrAdd).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
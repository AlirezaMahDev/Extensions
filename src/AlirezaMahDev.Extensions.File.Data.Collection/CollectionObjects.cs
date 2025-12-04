using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

internal class CollectionObjects : ICollectionObjects
{
    private readonly CollectionObjectFactory _objectFactory;
    private readonly ConcurrentDictionary<int, Lazy<ICollectionObject>> _cache = [];

    public CollectionObjects(CollectionAccess access, CollectionObjectFactory objectFactory)
    {
        _objectFactory = objectFactory;
        Access = access;
        LocationValue = Access.Location.GetOrAdd<CollectionObjectsValue>(".objects");
    }

    public ICollectionAccess Access { get; }
    public DataLocation<CollectionObjectsValue> LocationValue { get; }

    public ref String64 RefKey => ref LocationValue.RefKey;
    public ref CollectionObjectsValue RefValue => ref LocationValue.RefValue;
    public int Count => RefValue.Count;

    public bool TryGet(int index, [MaybeNullWhen(false)] out ICollectionObject item)
    {
        if (_cache.TryGetValue(index, out var lazy))
        {
            item = lazy.Value;
            return true;
        }

        item = this.FirstOrDefault(x => x.Index == index);
        if (item is not null)
        {
            _cache.TryAdd(index, new(item));
            return true;
        }

        return false;
    }

    public ICollectionObject GetOrAdd(int index)
    {
        return _cache.GetOrAdd(index,
                static (key, items) =>
                    new(() =>
                            items._objectFactory.GetOrCreate(new(items, key)),
                        LazyThreadSafetyMode.ExecutionAndPublication),
                this)
            .Value;
    }

    public ICollectionObject Add()
    {
        return GetOrAdd(Interlocked.Increment(ref RefValue.Count));
    }

    public bool TryRemove(int index, [MaybeNullWhen(false)] out ICollectionObject @object)
    {
        if (!TryGet(index, out @object))
        {
            return false;
        }

        @object.RefValue.DeleteAt = DateTimeOffset.Now;
        return true;
    }

    public virtual IEnumerator<ICollectionObject> GetEnumerator()
    {
        return RefValue.Count.ToIdRange().Select(GetOrAdd).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Save()
    {
        LocationValue.Save();
        foreach (var collectionObject in _objectFactory)
        {
            collectionObject.Save();
        }
    }
}
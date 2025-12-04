using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

public class CollectionPropertiesBase(IDataLocation location) : ICollectionProperties
{
    private readonly ConcurrentDictionary<String64, Lazy<ICollectionProperty>> _cache = [];

    protected virtual ICollectionProperty CreateProperty(String64 key)
    {
        return new CollectionPropertyBase(Location, key);
    }

    public IDataLocation Location { get; } = location.GetOrAdd(".properties");

    public ref String64 RefKey => ref Location.RefKey;

    public bool TryGet(String64 key, [MaybeNullWhen(false)] out ICollectionProperty property)
    {
        if (_cache.TryGetValue(key, out var lazy))
        {
            property = lazy.Value;
            return true;
        }

        property = this.FirstOrDefault(x => x.RefKey == key);
        if (property is not null)
        {
            _cache.TryAdd(key, new(property));
            return true;
        }

        return false;
    }

    public ICollectionProperty GetOrAdd(String64 key)
    {
        return _cache.GetOrAdd(key,
                static (key, items) =>
                    new(() => items.CreateProperty(key),
                        LazyThreadSafetyMode.ExecutionAndPublication),
                this)
            .Value;
    }

    public bool TryRemove(String64 key, [MaybeNullWhen(false)] out ICollectionProperty @object)
    {
        if (!TryGet(key, out @object))
        {
            return false;
        }

        @object.RefValue.DeleteAt = DateTimeOffset.Now;
        return true;
    }

    public virtual IEnumerator<ICollectionProperty> GetEnumerator()
    {
        return Location.Select(location => GetOrAdd(location.RefKey)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Save()
    {
        Location.Save();
        foreach (var keyValuePair in _cache)
        {
            keyValuePair.Value.Value.Save();
        }
    }
}
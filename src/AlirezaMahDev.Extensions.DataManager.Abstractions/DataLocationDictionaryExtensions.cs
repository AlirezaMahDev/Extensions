namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationDictionaryExtensions
{
    extension<TValue, TItem, TKey>(IDataDictionary<TValue, TItem, TKey> value)
        where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
        where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>
        where TKey : unmanaged, IEquatable<TKey>
    {
        public IDataDictionary<TValue, TItem, TKey> Dictionary() => null!;
    }

    extension<TValue, TKey>(IDataDictionaryTree<TValue, TKey> value)
        where TValue : unmanaged, IDataDictionaryTree<TValue, TKey>
        where TKey : unmanaged, IEquatable<TKey>
    {
        public IDataDictionary<TValue, TValue, TKey> TreeDictionary() => null!;
    }

    extension<TValue>(IDataCollectionItem<TValue> value)
        where TValue : unmanaged, IDataCollectionItem<TValue>
    {
        public IDataCollectionItem<TValue> CollectionItem() => null!;
    }

    extension<TValue, TItem, TKey>(DataWrap<TValue, IDataDictionary<TValue, TItem, TKey>> wrap)
        where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
        where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>, IDataValueDefault<TItem>
        where TKey : unmanaged, IEquatable<TKey>
    {
        public DataLocation<TItem> GetOrAdd(TKey key)
        {
            var locationWrapCollection = wrap
                .Wrap(x => x.Collection());
            return locationWrapCollection
                .GetChildren()
                .FirstOrDefault(x => x.GetRefValue(wrap.Access).Key.Equals(key))
                .WhenDefault(() =>
                    locationWrapCollection.Add(wrap.Access.Create(TItem.Default with { Key = key })));
        }

        public async ValueTask<DataLocation<TItem>> GetOrAddAsync(TKey key,
            CancellationToken cancellationToken = default)
        {
            var locationWrapCollection = wrap
                .Wrap(x => x.Collection());
            var dataLocation = await locationWrapCollection
                .GetChildren()
                .ToAsyncEnumerable()
                .FirstOrDefaultAsync(x => x.GetRefValue(wrap.Access).Key.Equals(key), cancellationToken);
            return await dataLocation.WhenDefaultAsync(async token =>
                    await locationWrapCollection.AddAsync(wrap.Access.Create(TItem.Default with { Key = key }),
                        cancellationToken: token),
                cancellationToken: cancellationToken);
        }
    }

    extension<TValue, TItem, TKey>(DataWrap<TValue, IDataDictionary<TValue, TItem, TKey>> wrap)
        where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
        where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>
        where TKey : unmanaged, IEquatable<TKey>
    {
        public DataLocation<TItem>? TryGet(TKey key)
        {
            return wrap
                .Wrap(x => x.Collection())
                .GetChildren()
                .FirstOrDefault(x => x.GetRefValue(wrap.Access).Key.Equals(key))
                .NullWhenDefault();
        }

        public async ValueTask<DataLocation<TItem>?> TryGetAsync(TKey key,
            CancellationToken cancellationToken = default)
        {
            var dataLocation = await wrap
                .Wrap(x => x.Collection())
                .GetChildren()
                .ToAsyncEnumerable()
                .FirstOrDefaultAsync(x => x.GetRefValue(wrap.Access).Key.Equals(key), cancellationToken);
            return dataLocation.NullWhenDefault();
        }

        public void Clear()
        {
            wrap
                .Wrap(x => x.Collection())
                .Clear();
        }

        public async ValueTask ClearAsync(CancellationToken cancellationToken = default)
        {
            await wrap
                .Wrap(x => x.Collection())
                .ClearAsync(cancellationToken);
        }

        public DataLocation<TItem>? Remove(TKey key)
        {
            var locationWrapCollection = wrap
                .Wrap(x => x.Collection());
            return locationWrapCollection
                .GetChildren()
                .FirstOrDefault(x => x.GetRefValue(wrap.Access).Key.Equals(key))
                .WhenNotDefault(x => locationWrapCollection.Remove(x));
        }

        public async ValueTask<DataLocation<TItem>?> RemoveAsync(TKey key,
            CancellationToken cancellationToken = default)
        {
            var locationWrapCollection = wrap
                .Wrap(x => x.Collection());
            var dataLocation = await locationWrapCollection
                .GetChildren()
                .ToAsyncEnumerable()
                .FirstOrDefaultAsync(x => x.GetRefValue(wrap.Access).Key.Equals(key),
                    cancellationToken: cancellationToken);
            return await dataLocation.WhenNotDefaultAsync(
                async (x, token) => await locationWrapCollection.RemoveAsync(x, token),
                cancellationToken);
        }

        public IEnumerable<DataLocation<TItem>> GetChildren()
        {
            return wrap
                .Wrap(x => x.Collection())
                .GetChildren();
        }
    }
}
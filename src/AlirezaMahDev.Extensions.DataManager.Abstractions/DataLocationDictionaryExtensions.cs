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

    extension<TValue, TItem, TKey>(DataLocationWrap<TValue, IDataDictionary<TValue, TItem, TKey>> locationWrap)
        where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
        where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>, IDataValueDefault<TItem>
        where TKey : unmanaged, IEquatable<TKey>
    {
        public DataLocation<TItem> GetOrAdd(TKey key)
        {
            var locationWrapCollection = locationWrap
                .Wrap(x => x.Collection());
            return locationWrapCollection
                .GetChildren()
                .FirstOrDefault(x => x.RefValue.Key.Equals(key))
                .WhenDefault(() => locationWrapCollection.Add(value => value.RefValue = value.RefValue with { Key = key }));
        }

        public async ValueTask<DataLocation<TItem>> GetOrAddAsync(TKey key,
            CancellationToken cancellationToken = default)
        {
            var locationWrapCollection = locationWrap
                .Wrap(x => x.Collection());
            var dataLocation = await locationWrapCollection
                .GetChildrenAsync(cancellationToken)
                .FirstOrDefaultAsync(x => x.RefValue.Key.Equals(key), cancellationToken);
            return await dataLocation.WhenDefaultAsync(async token =>
                    await locationWrapCollection.AddAsync(value => value.RefValue = value.RefValue with { Key = key }, cancellationToken: token),
                cancellationToken: cancellationToken);
        }
    }

    extension<TValue, TItem, TKey>(DataLocationWrap<TValue, IDataDictionary<TValue, TItem, TKey>> locationWrap)
        where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
        where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>
        where TKey : unmanaged, IEquatable<TKey>
    {
        public DataLocation<TItem>? TryGet(TKey key)
        {
            return locationWrap
                .Wrap(x => x.Collection())
                .GetChildren()
                .FirstOrDefault(x => x.RefValue.Key.Equals(key))
                .NullWhenDefault();
        }

        public async ValueTask<DataLocation<TItem>?> TryGetAsync(TKey key,
            CancellationToken cancellationToken = default)
        {
            var dataLocation = await locationWrap
                .Wrap(x => x.Collection())
                .GetChildrenAsync(cancellationToken)
                .FirstOrDefaultAsync(x => x.RefValue.Key.Equals(key), cancellationToken);
            return dataLocation.NullWhenDefault();
        }

        public void Clear()
        {
            locationWrap
                .Wrap(x => x.Collection())
                .Clear();
        }

        public async ValueTask ClearAsync(CancellationToken cancellationToken = default)
        {
            await locationWrap
                .Wrap(x => x.Collection())
                .ClearAsync(cancellationToken);
        }

        public DataLocation<TItem>? Remove(TKey key)
        {
            var locationWrapCollection = locationWrap
                .Wrap(x => x.Collection());
            return locationWrapCollection
                .GetChildren()
                .FirstOrDefault(x => x.RefValue.Key.Equals(key))
                .WhenNotDefault(x => locationWrapCollection.Remove(x));
        }

        public async ValueTask<DataLocation<TItem>?> RemoveAsync(TKey key,
            CancellationToken cancellationToken = default)
        {
            var locationWrapCollection = locationWrap
                .Wrap(x => x.Collection());
            var dataLocation = await locationWrapCollection
                .GetChildrenAsync(cancellationToken: cancellationToken)
                .FirstOrDefaultAsync(x => x.RefValue.Key.Equals(key), cancellationToken: cancellationToken);
            return await dataLocation.WhenNotDefaultAsync(
                async (x, token) => await locationWrapCollection.RemoveAsync(x, token),
                cancellationToken);
        }


        public IEnumerable<DataLocation<TItem>> GetChildren()
        {
            return locationWrap
                .Wrap(x => x.Collection())
                .GetChildren();
        }

        public IAsyncEnumerable<DataLocation<TItem>> GetChildrenAsync(CancellationToken cancellationToken = default)
        {
            return locationWrap
                .Wrap(x => x.Collection())
                .GetChildrenAsync(cancellationToken: cancellationToken);
        }
    }
}
namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationDictionaryExtensions
{
    extension<TValue, TItem, TKey>(IDataDictionary<TValue, TItem, TKey> value)
        where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
        where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>
        where TKey : unmanaged, IEquatable<TKey>
    {
        public IDataDictionary<TValue, TItem, TKey> Dictionary()
        {
            return null!;
        }
    }

    extension<TValue, TKey>(IDataDictionaryTree<TValue, TKey> value)
        where TValue : unmanaged, IDataDictionaryTree<TValue, TKey>
        where TKey : unmanaged, IEquatable<TKey>
    {
        public IDataDictionary<TValue, TValue, TKey> TreeDictionary()
        {
            return null!;
        }
    }

    extension<TValue>(IDataCollectionItem<TValue> value)
        where TValue : unmanaged, IDataCollectionItem<TValue>
    {
        public IDataCollectionItem<TValue> CollectionItem()
        {
            return null!;
        }
    }

    extension<TValue, TItem, TKey>(DataWrap<TValue, IDataDictionary<TValue, TItem, TKey>> wrap)
        where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
        where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>, IDataValueDefault<TItem>
        where TKey : unmanaged, IEquatable<TKey>
    {
        public DataLocation<TItem> GetOrAdd(TKey key)
        {
            var locationWrapCollection = wrap
                .Wrap([MethodImpl(MethodImplOptions.AggressiveInlining |
                                  MethodImplOptions.AggressiveOptimization)]
                    (x) => x.Collection());
            return locationWrapCollection
                .GetChildren()
                .FirstOrDefault([MethodImpl(MethodImplOptions.AggressiveInlining |
                                            MethodImplOptions.AggressiveOptimization)]
                    (x) =>
                        x.GetRefValue(wrap.Access).Key.Equals(key))
                .WhenDefault([MethodImpl(
                        MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                    () =>
                    {
                        wrap.Access.Create(TItem.Default with { Key = key }, out var location);
                        locationWrapCollection.Add(location);
                        return location;
                    });
        }
    }

    extension<TValue, TItem, TKey>(DataWrap<TValue, IDataDictionary<TValue, TItem, TKey>> wrap)
        where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
        where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>
        where TKey : unmanaged, IEquatable<TKey>
    {
        public Optional<DataLocation<TItem>> TryGet(TKey key)
        {
            return wrap
                .Wrap([MethodImpl(MethodImplOptions.AggressiveInlining |
                                  MethodImplOptions.AggressiveOptimization)]
                    (x) => x.Collection())
                .GetChildren()
                .FirstOrDefault([MethodImpl(MethodImplOptions.AggressiveInlining |
                                            MethodImplOptions.AggressiveOptimization)]
                    (x) =>
                        x.GetRefValue(wrap.Access).Key.Equals(key))
                .NullWhenDefault();
        }

        public async ValueTask<Optional<DataLocation<TItem>>> TryGetAsync(TKey key,
            CancellationToken cancellationToken = default)
        {
            var dataLocation = await wrap
                .Wrap([MethodImpl(MethodImplOptions.AggressiveInlining |
                                  MethodImplOptions.AggressiveOptimization)]
                    (x) => x.Collection())
                .GetChildren()
                .ToAsyncEnumerable()
                .FirstOrDefaultAsync(
                    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                    (x) => x.GetRefValue(wrap.Access).Key.Equals(key),
                    cancellationToken);
            return dataLocation.NullWhenDefault();
        }

        public void Clear()
        {
            wrap
                .Wrap([MethodImpl(MethodImplOptions.AggressiveInlining |
                                  MethodImplOptions.AggressiveOptimization)]
                    (x) => x.Collection())
                .Clear();
        }


        public Optional<DataLocation<TItem>> Remove(TKey key)
        {
            var locationWrapCollection = wrap
                .Wrap([MethodImpl(MethodImplOptions.AggressiveInlining |
                                  MethodImplOptions.AggressiveOptimization)]
                    (x) => x.Collection());
            return locationWrapCollection
                .GetChildren()
                .FirstOrDefault([MethodImpl(MethodImplOptions.AggressiveInlining |
                                            MethodImplOptions.AggressiveOptimization)]
                    (x) =>
                        x.GetRefValue(wrap.Access).Key.Equals(key))
                .WhenNotDefault([MethodImpl(MethodImplOptions.AggressiveInlining |
                                            MethodImplOptions.AggressiveOptimization)]
                    (x) =>
                        locationWrapCollection.Remove(x));
        }

        public IEnumerable<DataLocation<TItem>> GetChildren()
        {
            return wrap
                .Wrap([MethodImpl(MethodImplOptions.AggressiveInlining |
                                  MethodImplOptions.AggressiveOptimization)]
                    (x) => x.Collection())
                .GetChildren();
        }
    }
}
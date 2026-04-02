namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public class DataDictionaryWrap<TValue, TItem, TKey>(
    RefValueFunc<TValue, DataOffset> refChild,
    RefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyChild,
    RefValueFunc<TItem, DataOffset> refNext,
    RefReadOnlyValueFunc<TItem, DataOffset> refReadOnlyNext,
    RefValueFunc<TItem, TKey> refKey,
    RefReadOnlyValueFunc<TItem, TKey> refReadOnlyKey)
        : DataDictionaryWrap<TValue, TItem, TKey, DataDictionaryItemWrap<TItem, TKey>>(refChild, refReadOnlyChild,
        new(refNext, refReadOnlyNext, refKey, refReadOnlyKey))
    where TValue : unmanaged, IDataValue<TValue>
    where TItem : unmanaged, IDataValue<TItem>
{
}

public static class DataDictionaryWrapExtensions
{
    extension<TValue, TItem, TKey>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataValue<TValue>
        where TItem : unmanaged, IDataValue<TItem>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataDictionaryWrap<TValue, TItem, TKey> Dictionary(
            RefValueFunc<TValue, DataOffset> refChild,
            RefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyChild,
            RefValueFunc<TItem, DataOffset> refNext,
            RefReadOnlyValueFunc<TItem, DataOffset> refReadOnlyNext,
            RefValueFunc<TItem, TKey> refKey,
            RefReadOnlyValueFunc<TItem, TKey> refReadOnlyKey)
        {
            return new(
                refChild,
                refReadOnlyChild,
                refNext,
                refReadOnlyNext,
                refKey,
                refReadOnlyKey);
        }
    }

    extension<TValue, TItem, TKey>(IDataDictionary<TValue> value)
        where TValue : unmanaged, IDataDictionary<TValue>
        where TItem : unmanaged, IDataValue<TItem>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataDictionaryWrap<TValue, TItem, TKey> Dictionary(RefValueFunc<TItem, DataOffset> refNext,
            RefReadOnlyValueFunc<TItem, DataOffset> refReadOnlyNext,
            RefValueFunc<TItem, TKey> refKey,
            RefReadOnlyValueFunc<TItem, TKey> refReadOnlyKey)
        {
            return new(
                DataCollectionWrapChildDefault<TValue>.RefChild,
                DataCollectionWrapChildDefault<TValue>.RefReadOnlyChild,
                refNext,
                refReadOnlyNext,
                refKey,
                refReadOnlyKey);
        }
    }

    extension<TValue, TItem, TKey>(IDataDictionary<TValue, TItem> value)
        where TValue : unmanaged, IDataDictionary<TValue, TItem>
        where TItem : unmanaged, IDataDictionaryItem<TItem>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataDictionaryWrap<TValue, TItem, TKey> Dictionary(RefValueFunc<TItem, TKey> refKey,
            RefReadOnlyValueFunc<TItem, TKey> refReadOnlyKey)
        {
            return new(
                DataCollectionWrapChildDefault<TValue>.RefChild,
                DataCollectionWrapChildDefault<TValue>.RefReadOnlyChild,
                DataCollectionWrapNextDefault<TItem>.RefNext,
                DataCollectionWrapNextDefault<TItem>.RefReadOnlyNext,
                refKey,
                refReadOnlyKey);
        }
    }

    extension<TValue, TItem, TKey>(IDataDictionary<TValue, TItem, TKey> value)
        where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
        where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataDictionaryWrap<TValue, TItem, TKey> Dictionary()
        {
            return DataDictionaryWrapFullDefault<TValue, TItem, TKey>.Default;
        }
    }

    extension<TValue, TKey>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataValue<TValue>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataDictionaryItemWrap<TValue, TKey> DictionaryItem(RefValueFunc<TValue, DataOffset> refNext,
            RefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyNext,
            RefValueFunc<TValue, TKey> refKey,
            RefReadOnlyValueFunc<TValue, TKey> refReadOnlyKey)
        {
            return new(
                refNext,
                refReadOnlyNext,
                refKey,
                refReadOnlyKey);
        }
    }

    extension<TValue, TKey>(IDataDictionaryItem<TValue> value)
        where TValue : unmanaged, IDataDictionaryItem<TValue>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataDictionaryItemWrap<TValue, TKey> DictionaryItem(RefValueFunc<TValue, TKey> refKey,
            RefReadOnlyValueFunc<TValue, TKey> refReadOnlyKey)
        {
            return new(
                DataCollectionWrapNextDefault<TValue>.RefNext,
                DataCollectionWrapNextDefault<TValue>.RefReadOnlyNext,
                refKey,
                refReadOnlyKey);
        }
    }

    extension<TValue, TKey>(IDataDictionaryItem<TValue, TKey> value)
        where TValue : unmanaged, IDataDictionaryItem<TValue, TKey>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataDictionaryItemWrap<TValue, TKey> DictionaryItem()
        {
            return DataDictionaryItemWrapDefault<TValue, TKey>.Default;
        }
    }

    extension<TValue, TKey>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataValue<TValue>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataDictionaryWrap<TValue, TValue, TKey> TreeDictionary(
            RefValueFunc<TValue, DataOffset> getRefChildOrRefNext,
            RefReadOnlyValueFunc<TValue, DataOffset> getRefReAdOnlyChildOrReAdOnlyNext,
            RefValueFunc<TValue, TKey> refKey,
            RefReadOnlyValueFunc<TValue, TKey> refReadOnlyKey)
        {
            return new(getRefChildOrRefNext,
                getRefReAdOnlyChildOrReAdOnlyNext,
                getRefChildOrRefNext,
                getRefReAdOnlyChildOrReAdOnlyNext,
                refKey,
                refReadOnlyKey);
        }
    }
    extension<TValue, TKey>(IDataCollectionTree<TValue> value)
        where TValue : unmanaged, IDataCollectionTree<TValue>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataDictionaryWrap<TValue, TValue, TKey> TreeDictionary(
            RefValueFunc<TValue, TKey> refKey,
            RefReadOnlyValueFunc<TValue, TKey> refReadOnlyKey)
        {
            return new(
                DataCollectionWrapChildDefault<TValue>.RefChild,
                DataCollectionWrapChildDefault<TValue>.RefReadOnlyChild,
                DataCollectionWrapChildDefault<TValue>.RefChild,
                DataCollectionWrapChildDefault<TValue>.RefReadOnlyChild,
                refKey,
                refReadOnlyKey);
        }
    }

    extension<TValue, TKey>(IDataDictionaryTree<TValue, TKey> value)
        where TValue : unmanaged, IDataDictionaryTree<TValue, TKey>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataDictionaryWrap<TValue, TValue, TKey> TreeDictionary()
        {
            return DataDictionaryWrapFullDefault<TValue, TValue, TKey>.Default;
        }
    }

    extension<TValue, TItem, TKey>(in DataWrap<TValue, DataDictionaryWrap<TValue, TItem, TKey>> wrap)
        where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
        where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>, IDataValueDefault<TItem>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        public DataLocation<TItem> GetOrAdd(TKey key)
        {
            var locationWrapCollection = wrap.Wrap(x => x.Collection());
            var firstOrDefault = locationWrapCollection
                .GetChildren()
                .FirstOrDefault([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            (x) => x.ReadLock((scoped ref readonly y) => y.Key.Equals(in key)));

            if (!firstOrDefault.IsDefault)
            {
                return firstOrDefault;
            }

            wrap.Access.Create(TItem.Default with { Key = key }, out var location);
            locationWrapCollection.Add(in location);
            return location;
        }
    }

    extension<TValue, TItem, TKey>(in DataWrap<TValue, DataDictionaryWrap<TValue, TItem, TKey>> wrap)
        where TValue : unmanaged, IDataDictionary<TValue, TItem, TKey>
        where TItem : unmanaged, IDataDictionaryItem<TItem, TKey>
        where TKey : unmanaged, IScopedRefReadOnlyEquatable<TKey>
    {
        public Optional<DataLocation<TItem>> TryGet(TKey key)
        {
            var firstOrDefault = wrap.Wrap(x => x.Collection())
                .GetChildren()
                .FirstOrDefault([MethodImpl(MethodImplOptions.AggressiveInlining |
                                            MethodImplOptions.AggressiveOptimization)]
            (x) => x.ReadLock((scoped ref readonly y) => y.Key.Equals(in key)));
            return firstOrDefault.NullWhenDefault();
        }

        public void Clear()
        {
            var dataWrap = wrap.Wrap(x => x.Collection());
            dataWrap.Clear();
        }


        public Optional<DataLocation<TItem>> Remove(TKey key)
        {
            var locationWrapCollection = wrap.Wrap(x => x.Collection())
;
            var firstOrDefault = locationWrapCollection
                .GetChildren()
                .FirstOrDefault([MethodImpl(MethodImplOptions.AggressiveInlining |
                                            MethodImplOptions.AggressiveOptimization)]
            (x) => x.ReadLock((scoped ref readonly y) => y.Key.Equals(in key)));
            return firstOrDefault.IsDefault ? Optional<DataLocation<TItem>>.Null : locationWrapCollection.Remove(in firstOrDefault);
        }

        public IEnumerable<DataLocation<TItem>> GetChildren()
        {
            return wrap.Wrap(x => x.Collection())
                .GetChildren();
        }
    }
}
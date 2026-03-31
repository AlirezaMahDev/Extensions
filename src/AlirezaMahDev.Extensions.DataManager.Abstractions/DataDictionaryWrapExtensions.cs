namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public class DataDictionaryWrap<TValue, TItem, TKey>(
    ScopedRefValueFunc<TValue, DataOffset> refChild,
    ScopedRefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyChild,
    ScopedRefValueFunc<TItem, DataOffset> refNext,
    ScopedRefReadOnlyValueFunc<TItem, DataOffset> refReadOnlyNext,
    ScopedRefValueFunc<TItem, TKey> refKey,
    ScopedRefReadOnlyValueFunc<TItem, TKey> refReadOnlyKey)
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
            ScopedRefValueFunc<TValue, DataOffset> refChild,
            ScopedRefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyChild,
            ScopedRefValueFunc<TItem, DataOffset> refNext,
            ScopedRefReadOnlyValueFunc<TItem, DataOffset> refReadOnlyNext,
            ScopedRefValueFunc<TItem, TKey> refKey,
            ScopedRefReadOnlyValueFunc<TItem, TKey> refReadOnlyKey)
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
        public DataDictionaryWrap<TValue, TItem, TKey> Dictionary(ScopedRefValueFunc<TItem, DataOffset> refNext,
            ScopedRefReadOnlyValueFunc<TItem, DataOffset> refReadOnlyNext,
            ScopedRefValueFunc<TItem, TKey> refKey,
            ScopedRefReadOnlyValueFunc<TItem, TKey> refReadOnlyKey)
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
        public DataDictionaryWrap<TValue, TItem, TKey> Dictionary(ScopedRefValueFunc<TItem, TKey> refKey,
            ScopedRefReadOnlyValueFunc<TItem, TKey> refReadOnlyKey)
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
        public DataDictionaryItemWrap<TValue, TKey> DictionaryItem(ScopedRefValueFunc<TValue, DataOffset> refNext,
            ScopedRefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyNext,
            ScopedRefValueFunc<TValue, TKey> refKey,
            ScopedRefReadOnlyValueFunc<TValue, TKey> refReadOnlyKey)
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
        public DataDictionaryItemWrap<TValue, TKey> DictionaryItem(ScopedRefValueFunc<TValue, TKey> refKey,
            ScopedRefReadOnlyValueFunc<TValue, TKey> refReadOnlyKey)
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
            ScopedRefValueFunc<TValue, DataOffset> getRefChildOrRefNext,
            ScopedRefReadOnlyValueFunc<TValue, DataOffset> getRefReAdOnlyChildOrReAdOnlyNext,
            ScopedRefValueFunc<TValue, TKey> refKey,
            ScopedRefReadOnlyValueFunc<TValue, TKey> refReadOnlyKey)
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
            ScopedRefValueFunc<TValue, TKey> refKey,
            ScopedRefReadOnlyValueFunc<TValue, TKey> refReadOnlyKey)
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

    extension<TValue, TItem, TKey>(in DataLocationWrap<TValue, DataDictionaryWrap<TValue, TItem, TKey>> wrap)
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

    extension<TValue, TItem, TKey>(in DataLocationWrap<TValue, DataDictionaryWrap<TValue, TItem, TKey>> wrap)
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
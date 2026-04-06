namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataCollectionWrapExtensions
{
    extension<TValue, TItem>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataValue<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataCollectionWrap<TValue, TItem> Collection(
            RefValueFunc<TValue, DataOffset> refChild,
            RefReadOnlyValueFunc<TValue, DataOffset> refReadOnlyChild,
            RefValueFunc<TItem, DataOffset> refNext,
            RefReadOnlyValueFunc<TItem, DataOffset> refReadOnlyNext)
        {
            return new(refChild, refReadOnlyChild, refNext, refReadOnlyNext);
        }
    }

    extension<TValue, TItem>(IDataCollection<TValue> location)
        where TValue : unmanaged, IDataCollection<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataCollectionWrap<TValue, TItem> Collection(
            RefValueFunc<TItem, DataOffset> getRefNext,
            RefReadOnlyValueFunc<TItem, DataOffset> getRefReadOnlyNext)
        {
            return new(
                DataCollectionWrapChildDefault<TValue>.RefChild,
                DataCollectionWrapChildDefault<TValue>.RefReadOnlyChild,
                getRefNext,
                getRefReadOnlyNext);
        }
    }

    extension<TValue, TItem>(IDataCollection<TValue, TItem> value)
        where TValue : unmanaged, IDataCollection<TValue, TItem>
        where TItem : unmanaged, IDataCollectionItem<TItem>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataCollectionWrap<TValue, TItem> Collection()
        {
            return DataCollectionWrapFullDefault<TValue, TItem>.Default;
        }
    }

    extension<TValue>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataCollectionItemWrap<TValue> CollectionItem(RefValueFunc<TValue, DataOffset> getRefNext,
            RefReadOnlyValueFunc<TValue, DataOffset> getRefReadOnlyNext)
        {
            return new(getRefNext, getRefReadOnlyNext);
        }
    }

    extension<TValue>(IDataCollectionItem<TValue> value)
        where TValue : unmanaged, IDataCollectionItem<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataCollectionItemWrap<TValue> CollectionItem()
        {
            return DataCollectionItemWrapDefault<TValue, TValue>.Default;
        }
    }

    extension<TValue>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataCollectionWrap<TValue, TValue> TreeCollection(
            RefValueFunc<TValue, DataOffset> getRefChildOrRefNext,
            RefReadOnlyValueFunc<TValue, DataOffset> getRefReAdOnlyChildOrReAdOnlyNext)
        {
            return new(getRefChildOrRefNext,
                getRefReAdOnlyChildOrReAdOnlyNext,
                getRefChildOrRefNext,
                getRefReAdOnlyChildOrReAdOnlyNext);
        }
    }

    extension<TValue>(IDataCollectionTree<TValue> value)
        where TValue : unmanaged, IDataCollectionTree<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataCollectionWrap<TValue, TValue> TreeCollection()
        {
            return DataCollectionWrapFullDefault<TValue, TValue>.Default;
        }
    }

    extension<TValue>(in DataWrap<TValue, DataCollectionItemWrap<TValue>> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool GetNext(out DataLocation<TValue> result)
        {
            using var @lock = wrap.Location.ReadLock();
            var next = wrap.Wrap.RefReadOnlyNext(in @lock.RefReadOnlyValue);
            if (next.IsNull)
            {
                result = default;
                return false;
            }

            DataLocation<TValue>.Read(wrap.Location.Access,next, out result);
            return true;
        }
    }

    extension<TValue, TItem>(in DataWrap<TValue, DataCollectionWrap<TValue, TItem>> wrap)
        where TValue : unmanaged, IDataValue<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool GetChild(out DataLocation<TItem> result)
        {
            using var @lock = wrap.Location.ReadLock();
            var child = wrap.Wrap.RefReadOnlyChild(in @lock.RefReadOnlyValue);
            if (child.IsNull)
            {
                result = default;
                return false;
            }

            DataLocation<TItem>.Read(wrap.Location.Access,child, out result);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Add(in DataLocation<TItem> dataLocation)
        {
            using var parentLock = wrap.Location.WriteLock();
            using var childLock = dataLocation.WriteLock();
            wrap.Wrap.ItemWrap.RefNext(ref childLock.RefValue) = wrap.Wrap.RefReadOnlyChild(in parentLock.RefValue);
            wrap.Wrap.RefChild(ref parentLock.RefValue) = dataLocation.Offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Optional<DataLocation<TItem>> Remove(ref readonly DataOffset offset)
        {
            var previous = Optional<DataLocation<TItem>>.Null;
            foreach (var child in wrap.GetChildren())
            {
                if (child.Offset == offset)
                {
                    if (!previous.HasValue)
                    {
                        using var currentLock = wrap.Location.WriteLock();
                        using var childLock = child.ReadLock();
                        wrap.Wrap.RefChild(ref currentLock.RefValue) =
                            wrap.Wrap.ItemWrap.RefReadOnlyNext(in childLock.RefReadOnlyValue);
                    }
                    else
                    {
                        using var previousLock = previous.Value.WriteLock();
                        using var childLock = child.ReadLock();
                        wrap.Wrap.ItemWrap.RefNext(ref previousLock.RefValue) =
                            wrap.Wrap.ItemWrap.RefReadOnlyNext(in childLock.RefReadOnlyValue);
                    }

                    wrap.Location.Access.Trash.Add(child.Offset);
                    return child;
                }

                previous = child;
            }

            return Optional<DataLocation<TItem>>.Null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Optional<DataLocation<TItem>> Remove(in DataLocation<TItem> dataLocation)
        {
            return wrap.Remove(in dataLocation.Offset);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Clear()
        {
            using var dataLocations = wrap.GetChildren().ToMemoryList();
            foreach (ref var dataLocation in dataLocations.Memory.Span)
            {
                wrap.Remove(in dataLocation);
            }
        }
    }

    extension<TValue, TItem>(DataWrap<TValue, DataCollectionWrap<TValue, TItem>> wrap)
        where TValue : unmanaged, IDataValue<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IEnumerable<DataLocation<TItem>> GetChildren()
        {
            if (!wrap.GetChild(out var current))
            {
                yield break;
            }

            do
            {
                yield return current;
            } while (current.Wrap(wrap.Wrap.ItemWrap) is var dataWrap && dataWrap.GetNext(out current));
        }
    }
}
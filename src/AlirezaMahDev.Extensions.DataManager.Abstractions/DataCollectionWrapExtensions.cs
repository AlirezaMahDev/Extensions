namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataCollectionWrapExtensions
{
    extension<TValue, TItem>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataValue<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataCollectionWrap<TValue, TItem> Collection(
            GetRefValueFunc<TValue, DataOffset> getRefChildExpression,
            GetRefValueFunc<TItem, DataOffset> getRefNextExpression)
        {
            return new(getRefChildExpression, getRefNextExpression);
        }
    }

    private static class DataCollectionWrapChildDefault<TValue>
        where TValue : unmanaged, IDataCollection<TValue>
    {
        public static readonly GetRefValueFunc<TValue, DataOffset> GetRefChild =
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            (ref x) => ref x.Child;
    }

    private static class DataCollectionWrapNextDefault<TValue>
        where TValue : unmanaged, IDataCollectionItem<TValue>
    {
        public static readonly GetRefValueFunc<TValue, DataOffset> GetRefNext =
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            (ref x) => ref x.Next;
    }

    private static class DataCollectionWrapFullDefault<TValue, TItem>
        where TValue : unmanaged, IDataCollection<TValue>
        where TItem : unmanaged, IDataCollectionItem<TItem>
    {
        public static readonly DataCollectionWrap<TValue, TItem> Default
            = new(DataCollectionWrapChildDefault<TValue>.GetRefChild,
                DataCollectionWrapNextDefault<TItem>.GetRefNext);
    }

    extension<TValue, TItem>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataCollection<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataCollectionWrap<TValue, TItem> Collection(
            GetRefValueFunc<TItem, DataOffset> getRefNextExpression)
        {
            return new(DataCollectionWrapChildDefault<TValue>.GetRefChild, getRefNextExpression);
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

    extension<TValue>(IDataCollectionTree<TValue> value)
        where TValue : unmanaged, IDataCollectionTree<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IDataCollection<TValue, TValue> TreeCollection()
        {
            return null!;
        }
    }

    extension<TValue>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataCollectionItemWrap<TValue>
            CollectionItem(GetRefValueFunc<TValue, DataOffset> getRefNextExpression)
        {
            return new(getRefNextExpression);
        }
    }

    extension<TValue>(IDataCollectionItem<TValue> value)
        where TValue : unmanaged, IDataCollectionItem<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataCollectionItemWrap<TValue> CollectionItem()
        {
            return new(DataCollectionWrapNextDefault<TValue>.GetRefNext);
        }
    }

    extension<TValue>(ref readonly DataWrap<TValue, DataCollectionItemWrap<TValue>> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool GetNext(out DataLocation<TValue> result)
        {
            ref var next = ref wrap.Wrap.GetRefNext(ref wrap.Location.GetRefValue(wrap.Access));
            if (next.IsNull)
            {
                result = default;
                return false;
            }

            result = new(next);
            return true;
        }
    }

    extension<TValue, TItem>(ref readonly DataWrap<TValue, DataCollectionWrap<TValue, TItem>> wrap)
        where TValue : unmanaged, IDataLock<TValue>
        where TItem : unmanaged, IDataLock<TItem>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool GetChild(out DataLocation<TItem> result)
        {
            ref var child = ref wrap.Wrap.GetRefChild(ref wrap.Location.GetRefValue(wrap.Access));
            if (child.IsNull)
            {
                result = default;
                return false;
            }

            result = new(child);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Add(DataLocation<TItem> dataLocation)
        {
            var clearWrap = wrap.ClearWrap();
            using var lockWrap = clearWrap.Lock();
            var wrapData = dataLocation.Wrap(wrap.Access);
            using var lockData = wrapData.Lock();
            wrap.Wrap.ItemWrap.GetRefNext(ref wrapData.RefValue) = wrap.Wrap.GetChild(ref clearWrap.RefValue);
            wrap.Wrap.GetRefChild(ref clearWrap.RefValue) = dataLocation.Offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Optional<DataLocation<TItem>> Remove(in DataOffset offset)
        {
            var previous = Optional<DataLocation<TItem>>.Null;
            foreach (var dataLocation in wrap.GetChildren())
            {
                if (dataLocation.Offset == offset)
                {
                    if (previous.HasValue)
                    {
                        var previousWrap = previous.Value.Wrap(wrap.Access);
                        using var @lock = previousWrap.Lock();
                        wrap.Wrap.ItemWrap.GetRefNext(ref previousWrap.RefValue) =
                            wrap.Wrap.ItemWrap.GetValueNext(ref dataLocation.GetRefValue(wrap.Access));
                    }
                    else
                    {
                        var clearWrap = wrap.ClearWrap();
                        using var @lock = clearWrap.Lock();
                        wrap.Wrap.GetRefChild(ref clearWrap.RefValue) =
                            wrap.Wrap.ItemWrap.GetValueNext(ref dataLocation.GetRefValue(wrap.Access));
                    }

                    wrap.Access.GetTrash().Add(wrap.Access, dataLocation);
                    return dataLocation;
                }

                previous = dataLocation;
            }

            return Optional<DataLocation<TItem>>.Null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Optional<DataLocation<TItem>> Remove(in DataLocation<TItem> dataLocation)
        {
            return wrap.Remove(dataLocation.Offset);
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
        where TValue : unmanaged, IDataLock<TValue>
        where TItem : unmanaged, IDataLock<TItem>
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
            } while (current.Wrap(wrap.Access, wrap.Wrap.ItemWrap) is var dataWrap && dataWrap.GetNext(out current));
        }
    }
}
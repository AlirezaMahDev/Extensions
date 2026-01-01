using System.Linq.Expressions;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataCollectionWrapExtensions
{
    extension<TValue, TItem>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataValue<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        public DataCollectionWrap<TValue, TItem> Collection(
            Expression<SelectValueFunc<TValue, long>> selectChildExpression,
            Expression<SelectValueFunc<TItem, long>> selectNextExpression) =>
            new(selectChildExpression, selectNextExpression);
    }

    extension<TValue, TItem>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataCollection<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        public DataCollectionWrap<TValue, TItem> Collection(
            Expression<SelectValueFunc<TItem, long>> selectNextExpression) =>
            new(x => x.Child, selectNextExpression);
    }

    extension<TValue, TItem>(IDataCollection<TValue, TItem> value)
        where TValue : unmanaged, IDataCollection<TValue, TItem>
        where TItem : unmanaged, IDataCollectionItem<TItem>
    {
        public DataCollectionWrap<TValue, TItem> Collection() =>
            new(x => x.Child, x => x.Next);
    }

    extension<TValue>(IDataCollectionTree<TValue> value)
        where TValue : unmanaged, IDataCollectionTree<TValue>
    {
        public IDataCollection<TValue, TValue> TreeCollection() => null!;
    }

    extension<TValue>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public DataCollectionItemWrap<TValue>
            CollectionItem(Expression<SelectValueFunc<TValue, long>> selectNextExpression) =>
            new(selectNextExpression);
    }

    extension<TValue>(IDataCollectionItem<TValue> value)
        where TValue : unmanaged, IDataCollectionItem<TValue>
    {
        public DataCollectionItemWrap<TValue> CollectionItem() =>
            new(x => x.Next);
    }

    extension<TValue>(DataWrap<TValue, DataCollectionItemWrap<TValue>> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public DataLocation<TValue>? GetNext()
        {
            var next = wrap.Wrap.SelectNext(wrap.Location.GetRefValue(wrap.Access));
            return next == -1
                ? null
                : new DataLocation<TValue>(next);
        }
    }

    extension<TValue, TItem>(DataWrap<TValue, DataCollectionWrap<TValue, TItem>> wrap)
        where TValue : unmanaged, IDataValue<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        public DataLocation<TItem>? GetChild()
        {
            var child = wrap.Wrap.GetChild(wrap.Location.GetRefValue(wrap.Access));
            return child == -1
                ? null
                : new DataLocation<TItem>(child);
        }

        public IEnumerable<DataLocation<TItem>> GetChildren()
        {
            var current = wrap.GetChild();
            while (current.HasValue)
            {
                yield return current.Value;
                current = current.Value
                    .Wrap(wrap.Access, wrap.Wrap.ItemWrap)
                    .GetNext();
            }
        }

        public DataLocation<TItem> Add(DataLocation<TItem> dataLocation)
        {
            wrap.Wrap().Lock(location =>
                {
                    var child = wrap.Wrap.GetChild(location.GetRefValue(wrap.Access));

                    dataLocation.Wrap(wrap.Access)
                        .Lock(innerDataLocation =>
                        {
                            wrap.Wrap.ItemWrap.SetNext(ref innerDataLocation.GetRefValue(wrap.Access),
                                child);
                        });

                    wrap.Wrap.SetChild(ref location.GetRefValue(wrap.Access), dataLocation.Offset);
                });
            return dataLocation;
        }


        public async Task<DataLocation<TItem>> AddAsync(DataLocation<TItem> dataLocation,
            CancellationToken cancellationToken = default)
        {
            await wrap.Wrap()
                .LockAsync(async (location, token) =>
                    {
                        var child = wrap.Wrap.GetChild(location.GetRefValue(wrap.Access));

                        await dataLocation.Wrap(wrap.Access)
                            .LockAsync(innerDataLocation =>
                                {
                                    wrap.Wrap.ItemWrap.SetNext(
                                        ref innerDataLocation.GetRefValue(wrap.Access),
                                        child);
                                },
                                token);

                        wrap.Wrap.SetChild(ref location.GetRefValue(wrap.Access), dataLocation.Offset);
                    },
                    cancellationToken: cancellationToken);
            return dataLocation;
        }

        public DataLocation<TItem>? Remove(long offset)
        {
            DataLocation<TItem>? previous = null;
            foreach (var dataLocation in wrap.GetChildren())
            {
                if (dataLocation.Offset == offset)
                {
                    if (previous.HasValue)
                    {
                        previous.Value.Wrap(wrap.Access)
                            .Lock(location =>
                            {
                                wrap.Wrap.ItemWrap.SetNext(ref location.GetRefValue(wrap.Access),
                                    wrap.Wrap.ItemWrap.GetNext(dataLocation.GetRefValue(wrap.Access)));
                            });
                    }
                    else
                    {
                        wrap.Location.Wrap(wrap.Access)
                            .Lock(location =>
                            {
                                wrap.Wrap.SetChild(ref location.GetRefValue(wrap.Access),
                                    wrap.Wrap.ItemWrap.GetNext(dataLocation.GetRefValue(wrap.Access)));
                            });
                    }

                    wrap.Access.GetTrash().Add(wrap.Access, dataLocation);
                    return dataLocation;
                }

                previous = dataLocation;
            }

            return null;
        }

        public async ValueTask<DataLocation<TItem>?> RemoveAsync(long offset,
            CancellationToken cancellationToken = default)
        {
            DataLocation<TItem>? previous = null;
            foreach (var dataLocation in wrap.GetChildren())
            {
                if (dataLocation.Offset == offset)
                {
                    if (previous.HasValue)
                    {
                        await previous.Value.Wrap(wrap.Access)
                            .LockAsync(location =>
                                {
                                    wrap.Wrap.ItemWrap.SetNext(ref location.GetRefValue(wrap.Access),
                                        wrap.Wrap.ItemWrap.GetNext(
                                            dataLocation.GetRefValue(wrap.Access)));
                                },
                                cancellationToken);
                    }
                    else
                    {
                        await wrap.Location.Wrap(wrap.Access)
                            .LockAsync(location =>
                                {
                                    wrap.Wrap.SetChild(ref location.GetRefValue(wrap.Access),
                                        wrap.Wrap.ItemWrap.GetNext(
                                            dataLocation.GetRefValue(wrap.Access)));
                                },
                                cancellationToken);
                    }

                    await (await wrap.Access.GetTrashAsync(cancellationToken))
                        .AddAsync(wrap.Access, dataLocation, cancellationToken);
                    return dataLocation;
                }

                previous = dataLocation;
            }

            return null;
        }

        public DataLocation<TItem>? Remove(DataLocation<TItem> dataLocation) =>
            wrap.Remove(dataLocation.Offset);

        public async ValueTask<DataLocation<TItem>?> RemoveAsync(DataLocation<TItem> dataLocation,
            CancellationToken cancellationToken = default) =>
            await wrap.RemoveAsync(dataLocation.Offset, cancellationToken);


        public void Clear()
        {
            foreach (var dataLocation in wrap.GetChildren())
            {
                wrap.Remove(dataLocation);
            }
        }

        public async ValueTask ClearAsync(CancellationToken cancellationToken = default)
        {
            foreach (var dataLocation in wrap.GetChildren())
            {
                await wrap.RemoveAsync(dataLocation, cancellationToken);
            }
        }
    }
}
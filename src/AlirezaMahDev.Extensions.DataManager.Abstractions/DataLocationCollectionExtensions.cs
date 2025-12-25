using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationCollectionExtensions
{
    extension<TValue, TItem>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataValue<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        public DataCollection<TValue, TItem> Collection(
            Expression<SelectValueFunc<TValue, long>> selectChildExpression,
            Expression<SelectValueFunc<TItem, long>> selectNextExpression) =>
            new(selectChildExpression, selectNextExpression);
    }

    extension<TValue, TItem>(IDataValue<TValue> value)
        where TValue : unmanaged, IDataCollection<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        public DataCollection<TValue, TItem> Collection(
            Expression<SelectValueFunc<TItem, long>> selectNextExpression) =>
            new(x => x.Child, selectNextExpression);
    }

    extension<TValue, TItem>(IDataCollection<TValue, TItem> value)
        where TValue : unmanaged, IDataCollection<TValue, TItem>
        where TItem : unmanaged, IDataCollectionItem<TItem>
    {
        public DataCollection<TValue, TItem> Collection() =>
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
        public DataCollectionItem<TValue>
            CollectionItem(Expression<SelectValueFunc<TValue, long>> selectNextExpression) =>
            new(selectNextExpression);
    }

    extension<TValue>(IDataCollectionItem<TValue> value)
        where TValue : unmanaged, IDataCollectionItem<TValue>
    {
        public DataCollectionItem<TValue> CollectionItem() =>
            new(x => x.Next);
    }

    extension<TValue>(DataLocationWrap<TValue, DataCollectionItem<TValue>> locationWrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public DataLocation<TValue>? GetNext()
        {
            var next = locationWrap.Wrap.SelectNext(locationWrap.Location.RefValue);
            return next == -1
                ? null
                : locationWrap.Location.Access.Read<TValue>(next);
        }

        public async Task<DataLocation<TValue>?> GetNextAsync(CancellationToken cancellationToken = default)
        {
            var next = locationWrap.Wrap.SelectNext(locationWrap.Location.RefValue);
            return next == -1
                ? null
                : await locationWrap.Location.Access.ReadAsync<TValue>(next,
                    cancellationToken);
        }
    }

    extension<TValue, TItem>(DataLocationWrap<TValue, DataCollection<TValue, TItem>> locationWrap)
        where TValue : unmanaged, IDataCollection<TValue, TItem>
        where TItem : unmanaged, IDataCollectionItem<TItem>, IDataValueDefault<TItem>
    {
        public DataLocation<TItem> Add()
        {
            var dataLocation = locationWrap.Location.Access.Create<TItem>();
            return locationWrap.Add(dataLocation);
        }

        public DataLocation<TItem> Add(TItem @default)
        {
            var dataLocation = locationWrap.Location.Access.Create(@default);
            return locationWrap.Add(dataLocation);
        }

        public DataLocation<TItem> Add(DataLocationAction<TItem> action)
        {
            var dataLocation = locationWrap.Location.Access.Create(action);
            return locationWrap.Add(dataLocation);
        }

        public DataLocation<TItem> Add(DataLocationAction<TItem> action, TItem @default)
        {
            var dataLocation = locationWrap.Location.Access.Create(action, @default);
            return locationWrap.Add(dataLocation);
        }

        public async ValueTask<DataLocation<TItem>> AddAsync(CancellationToken cancellationToken = default)
        {
            var dataLocation = await locationWrap.Location.Access.CreateAsync<TItem>(cancellationToken);
            return locationWrap.Add(dataLocation);
        }

        public async ValueTask<DataLocation<TItem>> AddAsync(TItem @default,
            CancellationToken cancellationToken = default)
        {
            var dataLocation = await locationWrap.Location.Access.CreateAsync(@default, cancellationToken);
            return locationWrap.Add(dataLocation);
        }

        public async ValueTask<DataLocation<TItem>> AddAsync(DataLocationAction<TItem> action,
            CancellationToken cancellationToken = default)
        {
            var dataLocation = await locationWrap.Location.Access.CreateAsync(action, cancellationToken);
            return locationWrap.Add(dataLocation);
        }

        public async ValueTask<DataLocation<TItem>> AddAsync(DataLocationAction<TItem> action,
            TItem @default,
            CancellationToken cancellationToken = default)
        {
            var dataLocation = await locationWrap.Location.Access.CreateAsync(action, @default, cancellationToken);
            return locationWrap.Add(dataLocation);
        }

        public async ValueTask<DataLocation<TItem>> AddAsync(DataLocationAsyncAction<TItem> action,
            CancellationToken cancellationToken = default)
        {
            var dataLocation = await locationWrap.Location.Access.CreateAsync(action, cancellationToken);
            return locationWrap.Add(dataLocation);
        }

        public async ValueTask<DataLocation<TItem>> AddAsync(DataLocationAsyncAction<TItem> action,
            TItem @default,
            CancellationToken cancellationToken = default)
        {
            var dataLocation = await locationWrap.Location.Access.CreateAsync(action, @default, cancellationToken);
            return locationWrap.Add(dataLocation);
        }
    }

    extension<TValue, TItem>(DataLocationWrap<TValue, DataCollection<TValue, TItem>> locationWrap)
        where TValue : unmanaged, IDataValue<TValue>
        where TItem : unmanaged, IDataValue<TItem>
    {
        public DataLocation<TItem>? GetChild()
        {
            var child = locationWrap.Wrap.GetChild(locationWrap.Location.RefValue);
            return child == -1
                ? null
                : locationWrap.Location.Access.Read<TItem>(
                    child);
        }

        public async Task<DataLocation<TItem>?> GetChildAsync(CancellationToken cancellationToken = default)
        {
            var child = locationWrap.Wrap.GetChild(locationWrap.Location.RefValue);
            return child == -1
                ? null
                : await locationWrap.Location.Access.ReadAsync<TItem>(child,
                    cancellationToken);
        }

        public IEnumerable<DataLocation<TItem>> GetChildren()
        {
            var current = locationWrap.GetChild();
            while (current.HasValue)
            {
                yield return current.Value;
                current = current.Value
                    .Wrap(locationWrap.Wrap.ItemWrap)
                    .GetNext();
            }
        }

        public async IAsyncEnumerable<DataLocation<TItem>> GetChildrenAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var current = await locationWrap.GetChildAsync(cancellationToken);
            while (current.HasValue)
            {
                yield return current.Value;
                current = await current.Value
                    .Wrap(locationWrap.Wrap.ItemWrap)
                    .GetNextAsync(cancellationToken);
            }
        }

        public DataLocation<TItem> Add(DataLocation<TItem> dataLocation)
        {
            locationWrap.Location.Lock(location =>
            {
                var child = locationWrap.Wrap.GetChild(location.RefValue);

                dataLocation.Lock(innerDataLocation =>
                {
                    locationWrap.Wrap.ItemWrap.SetNext(ref innerDataLocation.RefValue, child);
                });

                locationWrap.Wrap.SetChild(ref location.RefValue, dataLocation.Offset);
            });
            return dataLocation;
        }

        public DataLocation<TItem>? Remove(long offset)
        {
            DataLocation<TItem>? previous = null;
            foreach (var dataLocation in locationWrap.GetChildren())
            {
                if (dataLocation.Offset == offset)
                {
                    if (previous.HasValue)
                    {
                        previous.Value.Lock(location =>
                        {
                            locationWrap.Wrap.ItemWrap.SetNext(ref location.RefValue,
                                locationWrap.Wrap.ItemWrap.GetNext(dataLocation.RefValue));
                        });
                    }
                    else
                    {
                        locationWrap.Location.Lock(location =>
                        {
                            locationWrap.Wrap.SetChild(ref location.RefValue,
                                locationWrap.Wrap.ItemWrap.GetNext(dataLocation.RefValue));
                        });
                    }

                    locationWrap.Location.Access.GetTrash().Add(dataLocation);
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
            await foreach (var dataLocation in locationWrap.GetChildrenAsync(
                               cancellationToken: cancellationToken))
            {
                if (dataLocation.Offset == offset)
                {
                    if (previous.HasValue)
                    {
                        previous.Value.Lock(location =>
                        {
                            locationWrap.Wrap.ItemWrap.SetNext(ref location.RefValue,
                                locationWrap.Wrap.ItemWrap.GetNext(dataLocation.RefValue));
                        });
                    }
                    else
                    {
                        locationWrap.Location.Lock(location =>
                        {
                            locationWrap.Wrap.SetChild(ref location.RefValue,
                                locationWrap.Wrap.ItemWrap.GetNext(dataLocation.RefValue));
                        });
                    }

                    await (await locationWrap.Location.Access.GetTrashAsync(cancellationToken))
                        .AddAsync(dataLocation, cancellationToken);
                    return dataLocation;
                }

                previous = dataLocation;
            }

            return null;
        }

        public DataLocation<TItem>? Remove(DataLocation<TItem> dataLocation) =>
            locationWrap.Remove(dataLocation.Offset);

        public async ValueTask<DataLocation<TItem>?> RemoveAsync(DataLocation<TItem> dataLocation,
            CancellationToken cancellationToken = default) =>
            await locationWrap.RemoveAsync(dataLocation.Offset, cancellationToken);


        public void Clear()
        {
            foreach (var dataLocation in locationWrap.GetChildren())
            {
                locationWrap.Remove(dataLocation);
            }
        }

        public async ValueTask ClearAsync(CancellationToken cancellationToken = default)
        {
            foreach (var dataLocation in locationWrap.GetChildren())
            {
                await locationWrap.RemoveAsync(dataLocation, cancellationToken);
            }
        }
    }
}
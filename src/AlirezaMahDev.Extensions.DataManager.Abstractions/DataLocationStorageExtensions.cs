namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationStorageExtensions
{
    extension<TValue>(IDataStorage<TValue> value)
        where TValue : unmanaged, IDataStorage<TValue>
    {
        public IDataStorage<TValue> Storage() => null!;
    }

    extension<TValue>(DataLocationWrap<TValue, IDataStorage<TValue>> locationWrap)
        where TValue : unmanaged, IDataStorage<TValue>
    {
        public DataLocation? TryGetData()
        {
            return locationWrap.Location.RefValue.Data == -1
                ? null
                : locationWrap.Location.Access.Read(locationWrap.Location.RefValue.Data, locationWrap.Location.RefValue.Size);
        }

        public async ValueTask<DataLocation?> TryGetDataAsync(CancellationToken cancellationToken = default)
        {
            return locationWrap.Location.RefValue.Data == -1
                ? null
                : await locationWrap.Location.Access.ReadAsync(
                    locationWrap.Location.RefValue.Data,
                    locationWrap.Location.RefValue.Size,
                    cancellationToken);
        }

        public DataLocation<TDataValue>? TryGetData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            return locationWrap.Location.RefValue.Data == -1
                ? null
                : TDataValue.ValueSize == locationWrap.Location.RefValue.Size
                    ? locationWrap.Location.Access.Read<TDataValue>(locationWrap.Location.RefValue.Data)
                    : throw new InvalidDataException($"{typeof(TDataValue).Name} size must be {locationWrap.Location.RefValue.Size}");
        }

        public async ValueTask<DataLocation<TDataValue>?> TryGetDataAsync<TDataValue>(
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            return locationWrap.Location.RefValue.Data == -1
                ? null
                : TDataValue.ValueSize == locationWrap.Location.RefValue.Size
                    ? await locationWrap.Location.Access.ReadAsync<TDataValue>(locationWrap.Location.RefValue.Data,
                        cancellationToken)
                    : throw new InvalidDataException($"{typeof(TDataValue).Name} size must be {locationWrap.Location.RefValue.Size}");
        }

        public DataLocation CreateNewData(int length)
        {
            var dataLocation = locationWrap.TryGetData();
            dataLocation?.Access.GetTrash().Add(dataLocation.Value);
            var newDataLocation = locationWrap.Location.Access.Create(length);
            locationWrap.Location.RefValue.Data = newDataLocation.Offset;
            locationWrap.Location.RefValue.Size = newDataLocation.Length;
            return newDataLocation;
        }

        public async ValueTask<DataLocation> CreateNewDataAsync(int length,
            CancellationToken cancellationToken = default)
        {
            var dataLocation = await locationWrap.TryGetDataAsync(cancellationToken);
            if (dataLocation.HasValue)
                await (await dataLocation.Value.Access.GetTrashAsync(cancellationToken))
                    .AddAsync(dataLocation.Value, cancellationToken);
            var newDataLocation = await locationWrap.Location.Access.CreateAsync(length, cancellationToken: cancellationToken);
            locationWrap.Location.RefValue.Data = newDataLocation.Offset;
            locationWrap.Location.RefValue.Size = newDataLocation.Length;
            return newDataLocation;
        }

        public DataLocation<TDataValue> CreateNewData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = locationWrap.TryGetData();
            dataLocation?.Access.GetTrash().Add(dataLocation.Value);
            var newDataLocation = locationWrap.Location.Access.Create<TDataValue>();
            locationWrap.Location.RefValue.Data = newDataLocation.Offset;
            locationWrap.Location.RefValue.Size = newDataLocation.Length;
            return newDataLocation;
        }

        public DataLocation<TDataValue> CreateNewData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = locationWrap.TryGetData();
            dataLocation?.Access.GetTrash().Add(dataLocation.Value);
            var newDataLocation = locationWrap.Location.Access.Create(@default);
            locationWrap.Location.RefValue.Data = newDataLocation.Offset;
            locationWrap.Location.RefValue.Size = newDataLocation.Length;
            return newDataLocation;
        }

        public async ValueTask<DataLocation<TDataValue>> CreateNewDataAsync<TDataValue>(
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = await locationWrap.TryGetDataAsync(cancellationToken);
            if (dataLocation.HasValue)
                await (await dataLocation.Value.Access.GetTrashAsync(cancellationToken))
                    .AddAsync(dataLocation.Value, cancellationToken);
            var newDataLocation = await locationWrap.Location.Access.CreateAsync<TDataValue>(cancellationToken);
            locationWrap.Location.RefValue.Data = newDataLocation.Offset;
            locationWrap.Location.RefValue.Size = newDataLocation.Length;
            return newDataLocation;
        }

        public async ValueTask<DataLocation<TDataValue>> CreateNewDataAsync<TDataValue>(
            TDataValue @default,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = await locationWrap.TryGetDataAsync(cancellationToken);
            if (dataLocation.HasValue)
                await (await dataLocation.Value.Access.GetTrashAsync(cancellationToken))
                    .AddAsync(dataLocation.Value, cancellationToken);
            var newDataLocation = await locationWrap.Location.Access.CreateAsync(@default, cancellationToken);
            locationWrap.Location.RefValue.Data = newDataLocation.Offset;
            locationWrap.Location.RefValue.Size = newDataLocation.Length;
            return newDataLocation;
        }

        public DataLocation GetOrCreateData(int length)
        {
            var dataLocation = locationWrap.TryGetData();
            return dataLocation.HasValue
                ? dataLocation.Value.Length == length
                    ? dataLocation.Value
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Value.Length} but you need {length}")
                : locationWrap.CreateNewData(length);
        }

        public async ValueTask<DataLocation> GetOrCreateDataAsync(int length,
            CancellationToken cancellationToken = default)
        {
            var dataLocation = await locationWrap.TryGetDataAsync(cancellationToken);
            return dataLocation.HasValue
                ? dataLocation.Value.Length == length
                    ? dataLocation.Value
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Value.Length} but you need {length}")
                : await locationWrap.CreateNewDataAsync(length, cancellationToken);
        }


        public DataLocation<TDataValue> GetOrCreateData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = locationWrap.TryGetData<TValue, TDataValue>();
            return dataLocation.HasValue
                ? dataLocation.Value.Length == TDataValue.ValueSize
                    ? dataLocation.Value
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Value.Length} but you need {TDataValue.ValueSize}")
                : locationWrap.CreateNewData<TValue, TDataValue>();
        }

        public DataLocation<TDataValue> GetOrCreateData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = locationWrap.TryGetData<TValue, TDataValue>();
            return dataLocation.HasValue
                ? dataLocation.Value.Length == TDataValue.ValueSize
                    ? dataLocation.Value
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Value.Length} but you need {TDataValue.ValueSize}")
                : locationWrap.CreateNewData(@default);
        }

        public async ValueTask<DataLocation<TDataValue>> GetOrCreateDataAsync<TDataValue>(
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = await locationWrap.TryGetDataAsync<TValue, TDataValue>(cancellationToken);
            return dataLocation.HasValue
                ? dataLocation.Value.Length == TDataValue.ValueSize
                    ? dataLocation.Value
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Value.Length} but you need {TDataValue.ValueSize}")
                : await locationWrap.CreateNewDataAsync<TValue, TDataValue>(cancellationToken);
        }

        public async ValueTask<DataLocation<TDataValue>> GetOrCreateDataAsync<TDataValue>(TDataValue @default,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = await locationWrap.TryGetDataAsync<TValue, TDataValue>(cancellationToken);
            return dataLocation.HasValue
                ? dataLocation.Value.Length == TDataValue.ValueSize
                    ? dataLocation.Value
                    : throw new InvalidDataException(
                        $"data length is {dataLocation.Value.Length} but you need {TDataValue.ValueSize}")
                : await locationWrap.CreateNewDataAsync(@default, cancellationToken);
        }


        public DataLocation GetOrCreateNewData(int length)
        {
            var dataLocation = locationWrap.TryGetData();
            return dataLocation.HasValue && dataLocation.Value.Length == length
                ? dataLocation.Value
                : locationWrap.CreateNewData(length);
        }

        public async ValueTask<DataLocation> GetOrCreateDataNewAsync(int length,
            CancellationToken cancellationToken = default)
        {
            var dataLocation = await locationWrap.TryGetDataAsync(cancellationToken);
            return dataLocation.HasValue && dataLocation.Value.Length == length
                ? dataLocation.Value
                : await locationWrap.CreateNewDataAsync(length, cancellationToken);
        }

        public DataLocation<TDataValue> GetOrCreateNewData<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = locationWrap.TryGetData<TValue, TDataValue>();
            return dataLocation.HasValue && dataLocation.Value.Length == TDataValue.ValueSize
                ? dataLocation.Value
                : locationWrap.CreateNewData<TValue, TDataValue>();
        }

        public DataLocation<TDataValue> GetOrCreateNewData<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = locationWrap.TryGetData<TValue, TDataValue>();
            return dataLocation.HasValue && dataLocation.Value.Length == TDataValue.ValueSize
                ? dataLocation.Value
                : locationWrap.CreateNewData(@default);
        }

        public async ValueTask<DataLocation<TDataValue>> GetOrCreateNewDataAsync<TDataValue>(
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = await locationWrap.TryGetDataAsync<TValue, TDataValue>(cancellationToken);
            return dataLocation.HasValue && dataLocation.Value.Length == TDataValue.ValueSize
                ? dataLocation.Value
                : await locationWrap.CreateNewDataAsync<TValue, TDataValue>(cancellationToken);
        }

        public async ValueTask<DataLocation<TDataValue>> GetOrCreateNewDataAsync<TDataValue>(
            TDataValue @default,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = await locationWrap.TryGetDataAsync<TValue, TDataValue>(cancellationToken);
            return dataLocation.HasValue && dataLocation.Value.Length == TDataValue.ValueSize
                ? dataLocation.Value
                : await locationWrap.CreateNewDataAsync(@default, cancellationToken);
        }
    }
}
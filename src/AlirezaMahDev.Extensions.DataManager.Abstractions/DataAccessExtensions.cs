namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataAccessExtensions
{
    extension(IDataAccess access)
    {
        public DataLocation Create(int length) =>
            DataLocation.Create(access, length);

        public async ValueTask<DataLocation> CreateAsync(
            int length,
            CancellationToken cancellationToken = default) =>
            await DataLocation.CreateAsync(access, length, cancellationToken);

        public DataLocation Read(long offset, int length) =>
            DataLocation.Read(access, offset, length);

        public async ValueTask<DataLocation>
            ReadAsync(long offset, int length, CancellationToken cancellationToken = default) =>
            await DataLocation.ReadAsync(access, offset, length, cancellationToken);

        public DataLocation<TDataValue> Create<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue> =>
            DataLocation<TDataValue>.Create(access);

        public DataLocation<TDataValue> Create<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            DataLocation<TDataValue>.Create(access, @default);

        public DataLocation<TDataValue> Create<TDataValue>(DataLocationAction<TDataValue> action)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue> =>
            access.Create<TDataValue>().Lock(action);

        public DataLocation<TDataValue> Create<TDataValue>(DataLocationAction<TDataValue> action, TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            access.Create(@default).Lock(action);

        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue> =>
            await DataLocation<TDataValue>.CreateAsync(access, cancellationToken);

        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(TDataValue @default,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            await DataLocation<TDataValue>.CreateAsync(access, @default, cancellationToken);

        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(DataLocationAction<TDataValue> action,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = await access.CreateAsync<TDataValue>(cancellationToken);
            return dataLocation.Lock(action);
        }

        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(DataLocationAction<TDataValue> action,
            TDataValue @default,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = await access.CreateAsync(@default, cancellationToken);
            return dataLocation.Lock(action);
        }

        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(
            DataLocationAsyncAction<TDataValue> action,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = await access.CreateAsync<TDataValue>(cancellationToken);
            return await dataLocation.LockAsync(action, cancellationToken);
        }

        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(
            DataLocationAsyncAction<TDataValue> action,
            TDataValue @default,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = await access.CreateAsync(@default, cancellationToken);
            return await dataLocation.LockAsync(action, cancellationToken);
        }

        public DataLocation<TDataValue> Read<TDataValue>(long offset)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            DataLocation<TDataValue>.Read(access, offset);

        public async ValueTask<DataLocation<TDataValue>> ReadAsync<TDataValue>(long offset,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            await DataLocation<TDataValue>.ReadAsync(access, offset, cancellationToken);
    }
}
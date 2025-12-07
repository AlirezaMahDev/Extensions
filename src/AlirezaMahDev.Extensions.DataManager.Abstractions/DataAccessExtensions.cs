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

        public void Write(DataLocation location) =>
            DataLocation.Write(access, location);

        public async ValueTask WriteAsync(DataLocation location, CancellationToken cancellationToken = default) =>
            await DataLocation.WriteAsync(access, location, cancellationToken);

        public DataLocation<TDataValue> Create<TDataValue>()
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue> =>
            DataLocation<TDataValue>.Create(access);

        public DataLocation<TDataValue> Create<TDataValue>(TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            DataLocation<TDataValue>.Create(access, @default);

        public DataLocation<TDataValue> Create<TDataValue>(Func<TDataValue, TDataValue> func)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue> =>
            access.Create<TDataValue>().Update(func);

        public DataLocation<TDataValue> Create<TDataValue>(Func<TDataValue, TDataValue> func, TDataValue @default)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            access.Create(@default).Update(func);

        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue> =>
            await DataLocation<TDataValue>.CreateAsync<TDataValue>(access, cancellationToken);

        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(TDataValue @default,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            await DataLocation<TDataValue>.CreateAsync(access, @default, cancellationToken);

        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(Func<TDataValue, TDataValue> func,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = await access.CreateAsync<TDataValue>( cancellationToken);
            return dataLocation.Update(func);
        }
        
        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(Func<TDataValue, TDataValue> func,
            TDataValue @default,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = await access.CreateAsync(@default, cancellationToken);
            return dataLocation.Update(func);
        }

        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(
            Func<TDataValue, CancellationToken, ValueTask<TDataValue>> func,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>, IDataValueDefault<TDataValue>
        {
            var dataLocation = await access.CreateAsync<TDataValue>( cancellationToken);
            return await dataLocation.UpdateAsync(func, cancellationToken);
        }
        
        public async ValueTask<DataLocation<TDataValue>> CreateAsync<TDataValue>(
            Func<TDataValue, CancellationToken, ValueTask<TDataValue>> func,
            TDataValue @default,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue>
        {
            var dataLocation = await access.CreateAsync(@default, cancellationToken);
            return await dataLocation.UpdateAsync(func, cancellationToken);
        }

        public DataLocation<TDataValue> Read<TDataValue>(long offset)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            DataLocation<TDataValue>.Read(access, offset);

        public async ValueTask<DataLocation<TDataValue>> ReadAsync<TDataValue>(long offset,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            await DataLocation<TDataValue>.ReadAsync(access, offset, cancellationToken);

        public void Write<TDataValue>(DataLocation<TDataValue> location)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            DataLocation<TDataValue>.Write(access, location);

        public async ValueTask WriteAsync<TDataValue>(DataLocation<TDataValue> location,
            CancellationToken cancellationToken = default)
            where TDataValue : unmanaged, IDataValue<TDataValue> =>
            await DataLocation<TDataValue>.WriteAsync(access, location, cancellationToken);
    }
}
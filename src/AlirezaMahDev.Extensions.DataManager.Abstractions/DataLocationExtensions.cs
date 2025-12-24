namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationExtensions
{
    extension<TSelf>(TSelf location)
        where TSelf : IDataLocationBase<TSelf>
    {
        public void Save()
        {
            TSelf.Write(location.Access, location);
        }

        public async ValueTask SaveAsync(CancellationToken cancellationToken = default)
        {
            await TSelf.WriteAsync(location.Access, location, cancellationToken);
        }
    }

    extension(DataLocation location)
    {
        public DataLocation<TValue> As<TValue>()
            where TValue : unmanaged, IDataValue<TValue> =>
            new(location);
    }

    extension<TValue>(DataLocation<TValue>)
        where TValue : unmanaged, IDataValue<TValue>, IDataValueDefault<TValue>
    {
        public static DataLocation<TValue> Create(IDataAccess access)
        {
            return DataLocation<TValue>.Create(access, TValue.Default);
        }

        public static async ValueTask<DataLocation<TValue>> CreateAsync(IDataAccess access,
            CancellationToken cancellationToken = default)
        {
            return await DataLocation<TValue>.CreateAsync(access, TValue.Default, cancellationToken);
        }
    }

    extension<TValue>(DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public DataLocation<TValue> Update(UpdateDataLocationAction<TValue> action)
        {
            location.Access.Lock(location.Offset);
            action(location);
            location.Access.UnLock(location.Offset);
            return location;
        }

        public async ValueTask<DataLocation<TValue>> UpdateAsync(
            UpdateDataLocationAsyncAction<TValue> action,
            CancellationToken cancellationToken = default)
        {
            location.Access.Lock(location.Offset);
            await action(location, cancellationToken);
            location.Access.UnLock(location.Offset);
            return location;
        }

        public bool IsDefault => location.Base.Length != TValue.ValueSize || location.RefValue.Equals(default);

        public DataLocation<TValue> WhenDefault(Func<DataLocation<TValue>> func) =>
            location.IsDefault ? func() : location;

        public async ValueTask<DataLocation<TValue>> WhenDefaultAsync(
            Func<CancellationToken, ValueTask<DataLocation<TValue>>> func,
            CancellationToken cancellationToken = default) =>
            location.IsDefault ? await func(cancellationToken) : location;

        public TResult? WhenNotDefault<TResult>(Func<DataLocation<TValue>, TResult> func) =>
            location.IsDefault ? func(location) : default;

        public async ValueTask<TResult?> WhenNotDefaultAsync<TResult>(
            Func<DataLocation<TValue>, CancellationToken, ValueTask<TResult?>> func,
            CancellationToken cancellationToken = default) =>
            location.IsDefault ? await func(location, cancellationToken) : default;


        public DataLocation<TValue>? NullWhenDefault() =>
            location.IsDefault ? null : location;
    }
}
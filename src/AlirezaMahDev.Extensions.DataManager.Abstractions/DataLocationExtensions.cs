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

        public LockScope LockScope() =>
            location.Access.LockScope(location.Offset);

        public async ValueTask<LockScope> LockScopeAsync(CancellationToken cancellationToken = default) =>
            await location.Access.LockScopeAsync(location.Offset, cancellationToken);
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
        public LockScope Lock() =>
            location.Access.LockScope(location.Offset);

        public async ValueTask<LockScope> LockAsync(CancellationToken cancellationToken = default) =>
            await location.Access.LockScopeAsync(location.Offset, cancellationToken);

        public DataLocation<TValue> Lock(DataLocationAction<TValue> action)
        {
            using var lockScope = location.Lock();
            action(location);
            return location;
        }

        public async ValueTask<DataLocation<TValue>> LockAsync(
            DataLocationAsyncAction<TValue> action,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = await location.LockAsync(cancellationToken);
            await action(location, cancellationToken);
            return location;
        }

        public TResult Lock<TResult>(DataLocationFunc<TValue, TResult> func)
        {
            using var lockScope = location.Lock();
            return func(location);
        }

        public async ValueTask<TResult> LockAsync<TResult>(
            DataLocationAsyncFunc<TValue, TResult> func,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = await location.LockAsync(cancellationToken);
            return await func(location, cancellationToken);
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
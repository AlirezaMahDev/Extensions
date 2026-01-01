namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLockWrapExtensions
{
    extension<TValue>(DataWrap<TValue> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public DataLockOffsetDisposable Lock() =>
            wrap.Access.Lock(wrap.Location.Offset);
        
        public DataWrap<TValue> Lock(DataLocationAction<TValue> action)
        {
            using var lockScope = wrap.Lock();
            action(wrap.Location);
            return wrap;
        }

        public TResult Lock<TResult>(DataLocationFunc<TValue, TResult> func)
        {
            using var lockScope = wrap.Lock();
            return func(wrap.Location);
        }

        public async ValueTask<DataLockOffsetDisposable> LockAsync(CancellationToken cancellationToken = default) =>
            await wrap.Access.LockAsync(wrap.Location.Offset, cancellationToken);

        public async ValueTask<DataWrap<TValue>> LockAsync(
            DataLocationAction<TValue> action,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = await wrap.LockAsync(cancellationToken);
            action(wrap.Location);
            return wrap;
        }
        
        public async ValueTask<DataWrap<TValue>> LockAsync(
            DataLocationAsyncAction<TValue> action,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = await wrap.LockAsync(cancellationToken);
            await action(wrap.Location, cancellationToken);
            return wrap;
        }
        
        public async ValueTask<TResult> LockAsync<TResult>(
            DataLocationFunc<TValue, TResult> func,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = await wrap.LockAsync(cancellationToken);
            return func(wrap.Location);
        }

        public async ValueTask<TResult> LockAsync<TResult>(
            DataLocationAsyncFunc<TValue, TResult> func,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = await wrap.LockAsync(cancellationToken);
            return await func(wrap.Location, cancellationToken);
        }
    }
}

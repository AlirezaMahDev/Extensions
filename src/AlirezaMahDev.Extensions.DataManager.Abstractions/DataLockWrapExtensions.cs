using System.IO.Hashing;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLockWrapExtensions
{
    private static readonly int SessionLockKey = (int)XxHash32.HashToUInt32(Guid.NewGuid().ToByteArray());

    extension<TValue>(DataWrap<TValue> wrap)
        where TValue : unmanaged, IDataLock<TValue>
    {
        public void UnLock()
        {
            Interlocked.Exchange(ref wrap.RefValue.Lock, 0);
        }

        public void FreeLastLock()
        {
            if (Volatile.Read(ref wrap.RefValue.Lock) != SessionLockKey)
                Interlocked.Exchange(ref wrap.RefValue.Lock, 0);
        }

        public DataLockDisposable<TValue> Lock()
        {
            wrap.FreeLastLock();
            while (Interlocked.CompareExchange(ref wrap.RefValue.Lock, SessionLockKey, 0) != 0)
            {
                Thread.Yield();
            }

            return new(wrap);
        }

        public async ValueTask<DataLockDisposable<TValue>> LockAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await ValueTask.FromCanceled<DataLockDisposable<TValue>>(cancellationToken);
            }

            wrap.FreeLastLock();

            if (cancellationToken.IsCancellationRequested)
            {
                return await ValueTask.FromCanceled<DataLockDisposable<TValue>>(cancellationToken);
            }

            while (Interlocked.CompareExchange(ref wrap.RefValue.Lock, SessionLockKey, 0) != 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return await ValueTask.FromCanceled<DataLockDisposable<TValue>>(cancellationToken);
                }

                await Task.Yield();

                if (cancellationToken.IsCancellationRequested)
                {
                    return await ValueTask.FromCanceled<DataLockDisposable<TValue>>(cancellationToken);
                }
            }

            return new(wrap);
        }

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
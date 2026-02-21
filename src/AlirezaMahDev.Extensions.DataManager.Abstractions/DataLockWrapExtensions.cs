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
            Interlocked.CompareExchange(ref wrap.RefValue.RefLock, 0, SessionLockKey);
        }

        public void FreeLastLock()
        {
            var read = Volatile.Read(ref wrap.RefValue.RefLock);
            if (read != 0 && read != SessionLockKey)
                Interlocked.CompareExchange(ref wrap.RefValue.RefLock, 0, read);
        }

        public DataLockDisposable<TValue> Lock()
        {
            wrap.FreeLastLock();
            while (Interlocked.CompareExchange(ref wrap.RefValue.RefLock, SessionLockKey, 0) != 0)
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

            while (Interlocked.CompareExchange(ref wrap.RefValue.RefLock, SessionLockKey, 0) != 0)
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

        public DataWrap<TValue> Lock(DataWrapAction<TValue> action)
        {
            using var lockScope = wrap.Lock();
            action(wrap);
            return wrap;
        }

        public TResult Lock<TResult>(DataWrapFunc<TValue, TResult> func)
        {
            using var lockScope = wrap.Lock();
            return func(wrap);
        }

        public async ValueTask<DataWrap<TValue>> LockAsync(
            DataWrapAction<TValue> action,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = await wrap.LockAsync(cancellationToken);
            action(wrap);
            return wrap;
        }

        public async ValueTask<DataWrap<TValue>> LockAsync(
            DataWrapAsyncAction<TValue> action,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = await wrap.LockAsync(cancellationToken);
            await action(wrap, cancellationToken);
            return wrap;
        }

        public async ValueTask<TResult> LockAsync<TResult>(
            DataWrapFunc<TValue, TResult> func,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = await wrap.LockAsync(cancellationToken);
            return func(wrap);
        }

        public async ValueTask<TResult> LockAsync<TResult>(
            DataWrapAsyncFunc<TValue, TResult> func,
            CancellationToken cancellationToken = default)
        {
            using var lockScope = await wrap.LockAsync(cancellationToken);
            return await func(wrap, cancellationToken);
        }
    }
}
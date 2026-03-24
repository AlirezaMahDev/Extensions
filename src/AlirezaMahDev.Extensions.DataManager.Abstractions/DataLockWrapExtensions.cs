using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLockWrapExtensions
{
    private static readonly int SessionLockKey =
        (int)XxHash32.HashToUInt32(Guid.NewGuid().ToByteArray()) | 1;

    extension<TValue>(ref readonly DataWrap<TValue> wrap)
        where TValue : unmanaged, IDataLock<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private void FreeLastLock()
        {
            var read = Volatile.Read(ref wrap.RefValue.Lock);
            if (read != 0 && read != SessionLockKey)
            {
                Interlocked.CompareExchange(
                    ref wrap.RefValue.Lock,
                    0,
                    read);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void UnLock()
        {
            Interlocked.CompareExchange(
                ref wrap.RefValue.Lock,
                0,
                SessionLockKey);
        }

        [MustDisposeResource]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLockDisposable<TValue> Lock()
        {
            wrap.FreeLastLock();

            SpinWait spinner = default;
            while (Interlocked.CompareExchange(
                       ref wrap.RefValue.Lock,
                       SessionLockKey,
                       0) !=
                   0)
            {
                spinner.SpinOnce();
            }

            return new(in wrap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Lock(DataWrapAction<TValue> action)
        {
            using var lockScope = wrap.Lock();
            action(in wrap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult Lock<TResult>(DataWrapFunc<TValue, TResult> func)
        {
            using var lockScope = wrap.Lock();
            return func(in wrap);
        }
    }
}
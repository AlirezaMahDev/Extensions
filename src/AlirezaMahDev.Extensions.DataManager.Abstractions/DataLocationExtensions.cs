namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationExtensions
{
    extension<TValue>(DataLocation<TValue>)
        where TValue : unmanaged, IDataValue<TValue>, IDataValueDefault<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void Create(IDataAccess access, out DataLocation<TValue> dataLocation)
        {
            DataLocation<TValue>.Create(access, TValue.Default, out dataLocation);
        }
    }

    extension<TValue>(in DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void UnsafeAccessRef(ScopedRefAction<TValue> action, CancellationToken cancellationToken = default)
        {
            using var cacheAccess = location.Access.GetCache(in location.Offset, cancellationToken);
            cacheAccess.Cache.AccessRefByte(location.Offset.Offset,
                (scoped ref b) =>
                    action(ref Unsafe.As<byte, TValue>(ref b)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult UnsafeAccessRef<TResult>(ScopedRefFunc<TValue, TResult> func,
            CancellationToken cancellationToken = default)
        {
            using var cacheAccess = location.Access.GetCache(in location.Offset, cancellationToken);
            return cacheAccess.Cache
                .AccessRefByte(location.Offset.Offset,
                    (scoped ref b) =>
                        func(ref Unsafe.As<byte, TValue>(ref b)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void UnsafeAccessRefReadOnly(ScopedRefReadOnlyAction<TValue> action,
            CancellationToken cancellationToken = default)
        {
            using var cacheAccess = location.Access.GetCache(in location.Offset, cancellationToken);
            cacheAccess.Cache
                .AccessRefReadOnlyByte(location.Offset.Offset,
                    (scoped ref readonly b) =>
                        action(ref Unsafe.As<byte, TValue>(ref Unsafe.AsRef(in b))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult UnsafeAccessRefReadOnly<TResult>(ScopedRefReadOnlyFunc<TValue, TResult> func,
            CancellationToken cancellationToken = default)
        {
            using var cacheAccess = location.Access.GetCache(in location.Offset, cancellationToken);
            return cacheAccess.Cache
                .AccessRefReadOnlyByte(location.Offset.Offset,
                    (scoped ref readonly b) =>
                        func(ref Unsafe.As<byte, TValue>(ref Unsafe.AsRef(in b))));
        }

        public bool IsDefault
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => location.Offset.IsDefault;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLocation<TValue> WhenDefault(Func<DataLocation<TValue>> func)
        {
            return location.IsDefault ? func() : location;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public TResult? WhenNotDefault<TResult>(Func<DataLocation<TValue>, TResult> func)
        {
            return location.IsDefault ? func(location) : default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Optional<DataLocation<TValue>> NullWhenDefault()
        {
            return location.IsDefault ? Optional<DataLocation<TValue>>.Null : location;
        }

        //get prime number from index 
    }
}
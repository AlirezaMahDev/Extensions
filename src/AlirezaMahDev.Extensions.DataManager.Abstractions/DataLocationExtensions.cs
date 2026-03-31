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
        public TValue CopyValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => location.ReadLock((scoped ref readonly x) => x);
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
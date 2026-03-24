namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationExtensions
{
    extension(ref readonly DataLocation location)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLocation<TValue> As<TValue>()
            where TValue : unmanaged, IDataValue<TValue>
        {
            return location.Offset.Length >= TValue.ValueSize
                ? new(location.Offset)
                : throw new InvalidCastException();
        }
    }

    extension<TValue>(DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ref TValue GetRefValue(IDataAccess access)
            => ref Unsafe.As<byte, TValue>(ref location.GetRef(access));
    }

    extension<TValue>(DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>, IDataValueDefault<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void Create(IDataAccess access, out DataLocation<TValue> dataLocation)
        {
            DataLocation<TValue>.Create(access, TValue.Default, out dataLocation);
        }
    }

    extension<TValue>(DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public bool IsDefault
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return location.Offset.IsDefault;
            }
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
    }
}
namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataEmptyWrapExtensions
{
    extension(DataWrap)
    {
        public static DataEmptyWrap Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => DataEmptyWrap.Default;
        }
    }

    extension<TValue>(DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, DataEmptyWrap> EmptyWrap(IDataAccess access)
        {
            return location.Wrap(access, DataWrap.Empty);
        }
    }

    extension<TValue, TWrap>(DataWrap<TValue, TWrap> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, DataEmptyWrap> EmptyWrap()
        {
            return new(wrap.Access, wrap.Location, DataEmptyWrap.Default);
        }
    }
}
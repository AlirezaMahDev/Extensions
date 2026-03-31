namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationNoWrapExtensions
{
    extension(DataLocationWrap)
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
        public DataLocationWrap<TValue, DataEmptyWrap> EmptyWrap(IDataAccess access)
        {
            return location.Wrap(access, DataLocationWrap.Empty);
        }
    }

    extension<TValue, TWrap>(DataLocationWrap<TValue, TWrap> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLocationWrap<TValue, DataEmptyWrap> EmptyWrap()
        {
            return new(wrap.Access, wrap.Location, DataEmptyWrap.Default);
        }
    }
}
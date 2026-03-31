namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationWrapExtensions
{
    extension<TValue, TWrap>(in DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLocationWrap<TValue, TWrap> Wrap(IDataAccess access, TWrap wrap)
        {
            return new(access, location, wrap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLocationWrap<TValue, TWrap> Wrap(IDataAccess access, Func<TValue, TWrap> wrap)
        {
            return new(access, location, wrap(default));
        }
    }

    extension<TValue, TWrap>(in DataLocationWrap<TValue, DataEmptyWrap> locationWrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLocationWrap<TValue, TWrap> Wrap(TWrap innerWrap)
        {
            return locationWrap.Location.Wrap(locationWrap.Access, innerWrap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLocationWrap<TValue, TWrap> Wrap(Func<TValue, TWrap> innerWrap)
        {
            return locationWrap.Location.Wrap(locationWrap.Access, innerWrap);
        }
    }

    extension<TValue, TWrap>(in DataLocationWrap<TValue, TWrap> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLocationWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(TInnerWrap innerWrap)
        {
            return wrap.Location.Wrap(wrap.Access, innerWrap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataLocationWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(Func<TValue, TInnerWrap> innerWrap)
        {
            return wrap.Location.Wrap(wrap.Access, innerWrap);
        }
    }
}
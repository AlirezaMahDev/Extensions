namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataWrapExtensions
{
    extension<TValue, TWrap>(in DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(TWrap wrap)
        {
            return new(location, wrap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(Func<TValue, TWrap> wrap)
        {
            return new(location, wrap(default));
        }
    }

    extension<TValue, TWrap>(in DataWrap<TValue, DataEmptyWrap> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(TWrap innerWrap)
        {
            return wrap.Location.Wrap(innerWrap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(Func<TValue, TWrap> innerWrap)
        {
            return wrap.Location.Wrap(innerWrap);
        }
    }

    extension<TValue, TWrap>(in DataWrap<TValue, TWrap> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(TInnerWrap innerWrap)
        {
            return wrap.Location.Wrap(innerWrap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(Func<TValue, TInnerWrap> innerWrap)
        {
            return wrap.Location.Wrap(innerWrap);
        }
    }
}
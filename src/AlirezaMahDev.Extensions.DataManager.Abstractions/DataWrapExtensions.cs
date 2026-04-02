namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataWrapExtensions
{
    extension<TValue, TWrap>(in DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(IDataAccess access, TWrap wrap)
        {
            return new(access, location, wrap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(IDataAccess access, Func<TValue, TWrap> wrap)
        {
            return new(access, location, wrap(default));
        }
    }

    extension<TValue, TWrap>(in DataWrap<TValue, DataEmptyWrap> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(TWrap innerWrap)
        {
            return wrap.Location.Wrap(wrap.Access, innerWrap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(Func<TValue, TWrap> innerWrap)
        {
            return wrap.Location.Wrap(wrap.Access, innerWrap);
        }
    }

    extension<TValue, TWrap>(in DataWrap<TValue, TWrap> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(TInnerWrap innerWrap)
        {
            return wrap.Location.Wrap(wrap.Access, innerWrap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(Func<TValue, TInnerWrap> innerWrap)
        {
            return wrap.Location.Wrap(wrap.Access, innerWrap);
        }
    }
}
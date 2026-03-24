namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataWrapExtensions
{
    extension<TValue>(ref readonly DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue> Wrap(IDataAccess access)
        {
            return new(access, location);
        }
    }

    extension<TValue, TWrap>(ref readonly DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(IDataAccess access, Func<TValue, TWrap> wrap)
        {
            return new(access, location, wrap(location.GetRefValue(access)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(IDataAccess access, TWrap wrap)
        {
            return new(access, location, wrap);
        }
    }

    extension<TValue>(ref readonly DataWrap<TValue> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public Memory<byte> Memory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return wrap.Location.GetMemory(wrap.Access);
            }
        }

        public ref TValue RefValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return ref wrap.Location.GetRefValue(wrap.Access);
            }
        }
    }

    extension<TValue, TWrap>(ref readonly DataWrap<TValue> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(Func<TValue, TWrap> func)
        {
            return new(wrap.Access, wrap.Location, func(wrap.Location.GetRefValue(wrap.Access)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TWrap> Wrap(TWrap innerWrap)
        {
            return new(wrap.Access, wrap.Location, innerWrap);
        }
    }

    extension<TValue, TWrap>(ref readonly DataWrap<TValue, TWrap> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue> ClearWrap()
        {
            return new(wrap.Access, wrap.Location);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(Func<TValue, TInnerWrap> func)
        {
            return wrap.Location.Wrap(wrap.Access, func);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public DataWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(TInnerWrap innerWrap)
        {
            return wrap.Location.Wrap(wrap.Access, innerWrap);
        }
    }
}
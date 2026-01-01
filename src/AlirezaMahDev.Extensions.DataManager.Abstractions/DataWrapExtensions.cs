namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataWrapExtensions
{
    extension<TValue>(DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public DataWrap<TValue> Wrap(IDataAccess access) =>
            new(access, location);
    }

    extension<TValue, TWrap>(DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public DataWrap<TValue, TWrap> Wrap(IDataAccess access, Func<TValue, TWrap> wrap) =>
            new(access, location, wrap(location.GetRefValue(access)));

        public DataWrap<TValue, TWrap> Wrap(IDataAccess access, TWrap wrap) =>
            new(access, location, wrap);
    }

    extension<TValue>(DataWrap<TValue> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public Memory<byte> Memory => wrap.Location.GetMemory(wrap.Access);
        public ref TValue RefValue => ref wrap.Location.GetRefValue(wrap.Access);
    }

    extension<TValue, TWrap>(DataWrap<TValue> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public DataWrap<TValue, TWrap> Wrap(Func<TValue, TWrap> func) =>
            new(wrap.Access, wrap.Location, func(wrap.Location.GetRefValue(wrap.Access)));

        public DataWrap<TValue, TWrap> Wrap(TWrap innerWrap) =>
            new(wrap.Access, wrap.Location, innerWrap);
    }

    extension<TValue, TWrap>(DataWrap<TValue, TWrap> wrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public DataWrap<TValue> Wrap() =>
            new(wrap.Access, wrap.Location);

        public DataWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(Func<TValue, TInnerWrap> func) =>
            wrap.Location.Wrap(wrap.Access, func);

        public DataWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(TInnerWrap innerWrap) =>
            wrap.Location.Wrap(wrap.Access, innerWrap);
    }
}
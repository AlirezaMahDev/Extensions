namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationWrapExtensions
{
    extension<TValue, TWrap>(DataLocation<TValue> location)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public DataLocationWrap<TValue, TWrap> Wrap(Func<TValue, TWrap> wrap) =>
            new(location, wrap(location.RefValue));
        
        public DataLocationWrap<TValue, TWrap> Wrap(TWrap wrap) =>
            new(location, wrap);
    }

    extension<TValue, TWrap>(DataLocationWrap<TValue, TWrap> locationWrap)
        where TValue : unmanaged, IDataValue<TValue>
    {
        public DataLocationWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(Func<TValue, TInnerWrap> wrap) =>
            locationWrap.Location.Wrap(wrap);
        
        public DataLocationWrap<TValue, TInnerWrap> Wrap<TInnerWrap>(TInnerWrap wrap) =>
            locationWrap.Location.Wrap(wrap);
    }
}
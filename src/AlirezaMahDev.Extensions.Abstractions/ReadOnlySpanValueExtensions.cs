namespace AlirezaMahDev.Extensions.Abstractions;

public static class ReadOnlySpanValueExtensions
{
    extension<T>(ref T t)
        where T : struct
    {
        public ReadOnlySpanValue<T> AsReadOnlySpanValue() =>
            new(in t);
    }
}
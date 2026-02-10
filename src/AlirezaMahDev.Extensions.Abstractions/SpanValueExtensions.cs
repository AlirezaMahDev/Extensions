namespace AlirezaMahDev.Extensions.Abstractions;

public static class SpanValueExtensions
{
    extension<T>(ref T t)
        where T : struct
    {
        public SpanValue<T> AsSpanValue() =>
            new(ref t);
    }

    extension<T>(SpanValue<T> spanValue)
        where T : struct
    {
        public ReadOnlySpanValue<T> AsReadOnlySpanValue() => spanValue;
    }
}
namespace AlirezaMahDev.Extensions.Abstractions;

public static class StringExtensions
{
    extension<T>(IEnumerable<T> values)
    {
        public string ToStringJoin(string separator = ",") =>
            string.Join(separator, values.Select(x => x?.ToString()));
    }

    extension<T>(ReadOnlySpan<T> values)
    {
        public string ToStringJoin(string separator = ",") =>
            string.Join(separator, values.Select((in x) => x?.ToString()));
    }
}
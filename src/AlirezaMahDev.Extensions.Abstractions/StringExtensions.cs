namespace AlirezaMahDev.Extensions.Abstractions;

public static class StringExtensions
{
    extension<T>(IEnumerable<T> values)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public string ToStringJoin(string separator = ",")
        {
            return string.Join(separator, values.Select(x => x?.ToString()));
        }
    }
}
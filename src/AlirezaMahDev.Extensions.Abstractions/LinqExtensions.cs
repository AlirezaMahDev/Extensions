namespace AlirezaMahDev.Extensions.Abstractions;

public static class LinqExtensions
{
    extension<T>(IAsyncEnumerable<T> asyncEnumerable)
    {
        public IAsyncEnumerable<T> AsAsyncEnumerable() => asyncEnumerable;
    }
}

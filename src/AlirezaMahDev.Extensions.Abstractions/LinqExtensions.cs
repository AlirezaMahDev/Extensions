namespace AlirezaMahDev.Extensions.Abstractions;

public static class LinqExtensions
{
    extension<T>(IAsyncEnumerable<T> asyncEnumerable)
    {
        public IAsyncEnumerable<T> AsAsyncEnumerable()
        {
            return asyncEnumerable;
        }

        public void Test()
        {
        }
    }
}
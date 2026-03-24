namespace AlirezaMahDev.Extensions.Abstractions;

public static class LinqExtensions
{
    extension<T>(IAsyncEnumerable<T> asyncEnumerable)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public IAsyncEnumerable<T> AsAsyncEnumerable()
        {
            return asyncEnumerable;
        }

        public void Test()
        {
        }
    }
}
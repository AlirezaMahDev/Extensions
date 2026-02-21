namespace AlirezaMahDev.Extensions.Abstractions;

public static class ReadOnlySpanExtensions
{
    extension<T>(ReadOnlySpan<T> readOnlySpan)
    {
        public IEnumerable<TOut> Select<TOut>(InFunc<T, TOut> inFunc)
        {
            var result = new TOut[readOnlySpan.Length];
            for (var index = 0; index < readOnlySpan.Length; index++)
            {
                result[index] = inFunc(in readOnlySpan[index]);
            }

            return result;
        }
    }
}
namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class Int64Extensions
{
    extension(long i)
    {
        public IEnumerable<long> ToIdRange()
        {
            for (long j = 1; j < i + 1; j++)
            {
                yield return j;
            }
        }

        public String64[] ToPathBinary()
        {
            return i.ToString().Select(x => (String64)x.ToString()).ToArray();
        }
    }
}
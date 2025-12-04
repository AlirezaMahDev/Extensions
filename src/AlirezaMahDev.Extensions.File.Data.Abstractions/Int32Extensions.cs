namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public static class Int32Extensions
{
    extension(int i)
    {
        public IEnumerable<int> ToIdRange()
        {
            for (var j = 1; j < i + 1; j++)
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
namespace AlirezaMahDev.Extensions.File.Data;

public static class Int64Extensions
{
    extension(long l)
    {
        public bool IsNotDefault => l != 0;
    }
}
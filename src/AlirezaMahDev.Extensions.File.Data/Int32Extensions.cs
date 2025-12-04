namespace AlirezaMahDev.Extensions.File.Data;

public static class Int32Extensions
{
    extension(int i)
    {
        public bool IsNotDefault => i != 0;
    }
}
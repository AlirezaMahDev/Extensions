namespace Parto.Extensions.Abstractions;

public static class Int32Extensions
{
    extension(int i)
    {
        public bool IsNotDefault => i != 0;
    }
}
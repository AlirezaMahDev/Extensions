namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CellLinkExtensions
{
    extension<T>(ref T value)
        where T : unmanaged, ICellLink<T>
    {
        public T Normalize() =>
            T.Normalize(in value);
    }
}
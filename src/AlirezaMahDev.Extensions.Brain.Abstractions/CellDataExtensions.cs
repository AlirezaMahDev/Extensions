namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class CellDataExtensions
{
    extension<T>(ref T value)
        where T : unmanaged, ICellData<T>
    {
        public T Normalize() =>
            T.Normalize(in value);
    }
}
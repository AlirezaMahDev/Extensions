namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellMemory<T> : IEnumerable<T>, ICollection<T>
{
    static abstract CellMemory<T> Empty { get; }
    Memory<T> Memory { get; }
    void Dispose();
}

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICellMemory<T> : IEnumerable<T>, ICollection<T>, IReadOnlyCollection<T>
{
    new int Count { get; }
    int ICollection<T>.Count => Count;
    int IReadOnlyCollection<T>.Count => Count;

    static abstract CellMemory<T> Empty { get; }
    Memory<T> Memory { get; }
    void Dispose();
}
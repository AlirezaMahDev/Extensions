namespace AlirezaMahDev.Extensions.Abstractions;

public interface IReadonlyMemoryList<T> : IDisposable, IReadOnlyList<T>
{
    ReadOnlyMemory<T> Memory { get; }
    IReadonlyMemoryList<T> Clone();
}
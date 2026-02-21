using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IMemoryList<T> : IList<T>, IReadonlyMemoryList<T>
{
    new int Count { get; }
    new Memory<T> Memory { get; }
    new MemoryList<T> Clone();

    new ref T this[int index]
    {
        get;
    }

    T IList<T>.this[int index]
    {
        get => this[index];
        set => this[index] = value;
    }

    T IReadOnlyList<T>.this[int index] => this[index];

    ReadOnlyMemory<T> IReadonlyMemoryList<T>.Memory => Memory;

    [MustDisposeResource]
    IReadonlyMemoryList<T> IReadonlyMemoryList<T>.Clone() => Clone();

    int ICollection<T>.Count => Count;

    int IReadOnlyCollection<T>.Count => Count;
}
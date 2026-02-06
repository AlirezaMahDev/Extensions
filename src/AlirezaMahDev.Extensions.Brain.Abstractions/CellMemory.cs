using System.Buffers;
using System.Collections;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

// ReSharper disable once RedundantExtendsListEntry
[MustDisposeResource]
// ReSharper disable once RedundantExtendsListEntry
public sealed class CellMemory<T> : IDisposable, IEnumerable<T>, ICollection<T>
{
    public static CellMemory<T> Empty { get; } = new(CellEnumerable<T>.Empty);

    private IMemoryOwner<T>? _memoryOwner;

    public CellMemory(CellEnumerable<T> cellEnumerable)
    {
        if (cellEnumerable.Count == 0)
        {
            Memory = new();
            return;
        }

        _memoryOwner = MemoryPool<T>.Shared.Rent(cellEnumerable.Count);
        Memory = _memoryOwner.Memory[..cellEnumerable.Count];

        var index = 0;
        var span = Memory.Span;
        foreach (var item in cellEnumerable)
        {
            span[index] = item;
            index++;
        }

        if (index != cellEnumerable.Count)
        {
            Dispose();
            throw new IndexOutOfRangeException($"Expected {cellEnumerable.Count} items but received {index}.");
        }
    }

    public Memory<T> Memory { get; }

    public int Count => Memory.Length;
    public bool IsReadOnly => true;

    public void Dispose()
    {
        _memoryOwner?.Dispose();
        _memoryOwner = null;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Memory.Length; ++i)
            yield return Memory.Span[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Contains(T item)
    {
        return Memory.Span.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Memory.Span.CopyTo(array.AsSpan(arrayIndex));
    }

    void ICollection<T>.Add(T item) => throw new NotSupportedException();
    void ICollection<T>.Clear() => throw new NotSupportedException();
    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
}
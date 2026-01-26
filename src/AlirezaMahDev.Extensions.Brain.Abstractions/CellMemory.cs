using System.Buffers;
using System.Collections;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[MustDisposeResource]
// ReSharper disable once RedundantExtendsListEntry
public sealed class CellMemory<T> : IDisposable, IEnumerable<T>, ICollection<T>
{
    [MustDisposeResource]
    public static CellMemory<T> Create(int count, IEnumerable<T> enumerable)
    {
        var cellMemory = new CellMemory<T>(count);
        if (count == 0)
            return cellMemory;

        var index = 0;
        foreach (var item in enumerable)
        {
            cellMemory.Memory.Span[index] = item;
            index++;
        }

        if (index != count)
            throw new IndexOutOfRangeException($"Expected {count} items but received {index}.");

        return cellMemory;
    }

    private readonly IMemoryOwner<T>? _memoryOwner;

    public CellMemory(int count)
    {
        if (count != 0)
        {
            _memoryOwner = MemoryPool<T>.Shared.Rent(count);
            Memory = _memoryOwner.Memory[..count];
        }
        else
        {
            Memory = new();
        }
    }

    public Memory<T> Memory { get; }

    public int Count => Memory.Length;
    public bool IsReadOnly => true;

    public void Dispose()
    {
        _memoryOwner?.Dispose();
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
        throw new NotImplementedException();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    void ICollection<T>.Add(T item) => throw new NotSupportedException();
    void ICollection<T>.Clear() => throw new NotSupportedException();
    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
}
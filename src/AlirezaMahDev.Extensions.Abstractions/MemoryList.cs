using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

[MustDisposeResource]
[DebuggerDisplay("Count = {Count}")]
public sealed class MemoryList<T>(int capacity = -1) : IDisposable, IList<T>
{
    public MemoryList(int capacity, IEnumerable<T> values) : this(capacity)
    {
        foreach (var value in values)
        {
            Add(value);
        }
    }
    public MemoryList(IEnumerable<T> values) : this(-1, values)
    {
    }

    private bool _disposedValue;

    private IMemoryOwner<T> MemoryOwner { get; set; } = MemoryPool<T>.Shared.Rent(capacity);
    private Memory<T> OriginalMemory => MemoryOwner.Memory;
    private int OriginalCount => OriginalMemory.Length;

    public Memory<T> Memory => OriginalMemory[..Count];
    public int Count { get; private set; }

    public bool IsReadOnly => false;

    public IEnumerator<T> GetEnumerator()
    {
        return MemoryMarshal.ToEnumerable<T>(Memory).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void CheckNeedGrow()
    {
        if (OriginalCount < Count)
        {
            using var lastMemoryOwner = MemoryOwner;
            MemoryOwner = MemoryPool<T>.Shared.Rent(Count);
            lastMemoryOwner.Memory.CopyTo(OriginalMemory);
        }
    }

    public void Add(T item)
    {
        Count++;
        CheckNeedGrow();
        Memory.Span[Count - 1] = item;
    }

    public void Clear()
    {
        Count = 0;
        OriginalMemory.Span.Clear();
    }

    public bool Contains(T item)
    {
        return IndexOf(item) != -1;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (int i = 0; i < Count; i++)
        {
            array[arrayIndex + i] = this[i];
        }
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index == -1)
            return false;
        RemoveAt(index);
        return true;
    }

    public int IndexOf(T item)
    {
        return Memory.Span.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        Count++;
        CheckNeedGrow();
        Memory[index..^1].CopyTo(OriginalMemory[(index + 1)..]);
        Memory.Span[index] = item;
    }

    public void RemoveAt(int index)
    {
        Count--;
        if (index != Count - 1)
            OriginalMemory[(index + 1)..].CopyTo(OriginalMemory[index..]);
    }

    public T this[int index]
    {
        get => Memory.Span[index];
        set => Memory.Span[index] = value;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                MemoryOwner.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
    }
}
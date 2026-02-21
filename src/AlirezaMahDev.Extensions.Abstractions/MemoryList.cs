using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

[MustDisposeResource]
[DebuggerDisplay("Count = {Count}")]
public sealed class MemoryList<T>(int capacity = -1) : IMemoryList<T>
{
    [MustDisposeResource]
    public static implicit operator MemoryList<T>(ReadOnlySpan<T> values) => new(values);

    [MustDisposeResource]
    public static implicit operator MemoryList<T>(Span<T> values) => new(values);

    [MustDisposeResource]
    public static implicit operator MemoryList<T>(ReadOnlyMemory<T> values) => new(values);

    [MustDisposeResource]
    public static implicit operator MemoryList<T>(Memory<T> values) => new(values);

    [MustDisposeResource]
    public static MemoryList<T> Create(int length)
    {
        var memoryList = new MemoryList<T>(length);
        memoryList.Count = length;
        return memoryList;
    }

    public MemoryList(ReadOnlySpan<T> values) : this(values.Length)
    {
        Count = values.Length;
        values.CopyTo(Memory.Span);
    }

    public MemoryList(ReadOnlyMemory<T> values) : this(values.Length)
    {
        Count = values.Length;
        values.CopyTo(Memory);
    }

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
        Memory.CopyTo(array.AsMemory());
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

    [MustDisposeResource]
    public MemoryList<T> Clone()
    {
        MemoryList<T> memoryList = MemoryList<T>.Create(Count);
        Memory.CopyTo(memoryList.Memory);
        return memoryList;
    }

    public ref T this[int index]
    {
        get => ref Memory.Span[index];
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
using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace AlirezaMahDev.Extensions.Abstractions;

[MustDisposeResource]
[DebuggerDisplay("Count = {Count}")]
public class MemoryList<T>(int capacity = -1) : IDisposable, IList<T>
{
    private bool _disposedValue;

    protected virtual IMemoryOwner<T> MemoryOwner { get; set; } = MemoryPool<T>.Shared.Rent(capacity);
    protected virtual Memory<T> OriginalMemory => MemoryOwner.Memory;
    protected virtual int OriginalCount => OriginalMemory.Length;

    public virtual Memory<T> Memory => OriginalMemory[..Count];
    public virtual int Count { get; private set; }

    public virtual bool IsReadOnly => false;

    public virtual IEnumerator<T> GetEnumerator()
    {
        return MemoryMarshal.ToEnumerable<T>(Memory).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    protected virtual void CheckNeedGrow()
    {
        if (OriginalCount < Count)
        {
            using var lastMemoryOwner = MemoryOwner;
            MemoryOwner = MemoryPool<T>.Shared.Rent(Count);
            lastMemoryOwner.Memory.CopyTo(OriginalMemory);
        }
    }

    public virtual void Add(T item)
    {
        Count++;
        CheckNeedGrow();
        Memory.Span[Count - 1] = item;
    }

    public virtual void Clear()
    {
        Count = 0;
        OriginalMemory.Span.Clear();
    }

    public virtual bool Contains(T item)
    {
        return IndexOf(item) != -1;
    }

    public virtual void CopyTo(T[] array, int arrayIndex)
    {
        for (int i = 0; i < Count; i++)
        {
            array[arrayIndex + i] = this[i];
        }
    }

    public virtual bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index == -1)
            return false;
        RemoveAt(index);
        return true;
    }

    public virtual int IndexOf(T item)
    {
        return Memory.Span.IndexOf(item);
    }

    public virtual void Insert(int index, T item)
    {
        Count++;
        CheckNeedGrow();
        Memory[index..^1].CopyTo(OriginalMemory[(index + 1)..]);
        Memory.Span[index] = item;
    }

    public virtual void RemoveAt(int index)
    {
        Count--;
        if (index != Count - 1)
            OriginalMemory[(index + 1)..].CopyTo(OriginalMemory[index..]);
    }

    public virtual T this[int index]
    {
        get => Memory.Span[index];
        set => Memory.Span[index] = value;
    }

    protected virtual void Dispose(bool disposing)
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
        GC.SuppressFinalize(this);
    }
}
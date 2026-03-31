using System.Buffers;
using System.Collections;
using System.Diagnostics;

namespace AlirezaMahDev.Extensions.Abstractions;

[MustDisposeResource]
[DebuggerDisplay("Count = {Count}")]
[CollectionBuilder(typeof(MemoryListBuilder), nameof(MemoryListBuilder.Create))]
public sealed class MemoryList<T>(int capacity = -1) : IMemoryList<T>
{
    [MustDisposeResource]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator MemoryList<T>(ReadOnlySpan<T> values)
    {
        return [.. values];
    }

    [MustDisposeResource]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator MemoryList<T>(Span<T> values)
    {
        return [.. values];
    }

    [MustDisposeResource]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator MemoryList<T>(ReadOnlyMemory<T> values)
    {
        return [.. values.Span];
    }

    [MustDisposeResource]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator MemoryList<T>(Memory<T> values)
    {
        return [.. values.Span];
    }

    [MustDisposeResource]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static MemoryList<T> Create(int length)
    {
        return new(length) { Count = length };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public MemoryList(ReadOnlySpan<T> values) : this(values.Length)
    {
        Count = values.Length;
        values.CopyTo(Memory.Span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public MemoryList(int capacity, ReadOnlySpan<T> values) : this(Math.Max(capacity, values.Length))
    {
        Count = values.Length;
        values.CopyTo(Memory.Span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public MemoryList(int capacity, IEnumerable<T> values) : this(capacity)
    {
        foreach (var value in values)
        {
            Add(value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public MemoryList(IEnumerable<T> values) : this(-1, values)
    {
    }

    private bool _disposedValue;

    private IMemoryOwner<T> MemoryOwner
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set;
    } = MemoryPool<T>.Shared.Rent(capacity);

    private Memory<T> OriginalMemory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => MemoryOwner.Memory;
    }

    private int OriginalCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => OriginalMemory.Length;
    }

    public Memory<T> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => OriginalMemory[..Count];
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private set;
    }

    public bool IsReadOnly => false;

    public IEnumerator<T> GetEnumerator()
    {
        return MemoryMarshal.ToEnumerable<T>(Memory).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void CheckNeedGrow()
    {
        if (OriginalCount < Count)
        {
            using var lastMemoryOwner = MemoryOwner;
            MemoryOwner = MemoryPool<T>.Shared.Rent(Count);
            lastMemoryOwner.Memory.CopyTo(OriginalMemory);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Add(T item)
    {
        Count++;
        CheckNeedGrow();
        Memory.Span[Count - 1] = item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clear()
    {
        Count = 0;
        OriginalMemory.Span.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Contains(T item)
    {
        return IndexOf(item) != -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void CopyTo(T[] array, int arrayIndex)
    {
        Memory.CopyTo(array.AsMemory());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index == -1)
        {
            return false;
        }

        RemoveAt(index);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int IndexOf(T item)
    {
        return Memory.Span.IndexOf(item);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Insert(int index, T item)
    {
        Count++;
        CheckNeedGrow();
        Memory[index..^1].CopyTo(OriginalMemory[(index + 1)..]);
        Memory.Span[index] = item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void RemoveAt(int index)
    {
        Count--;
        if (index != Count - 1)
        {
            OriginalMemory[(index + 1)..].CopyTo(OriginalMemory[index..]);
        }
    }

    [MustDisposeResource]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public MemoryList<T> Clone()
    {
        var memoryList = Create(Count);
        Memory.CopyTo(memoryList.Memory);
        return memoryList;
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref Memory.Span[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        Dispose(true);
    }
}
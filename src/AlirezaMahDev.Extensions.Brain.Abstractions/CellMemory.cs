namespace AlirezaMahDev.Extensions.Brain.Abstractions;

[MustDisposeResource]
public sealed class CellMemory<T> : IDisposable, IEnumerable<T>, ICollection<T>, ICellMemory<T>
{
    public static CellMemory<T> Empty { get; } = new(CellEnumerable<T>.Empty);

    private IMemoryOwner<T>? _memoryOwner;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

    public Memory<T> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => Memory.Length;
    }

    public bool IsReadOnly
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _memoryOwner?.Dispose();
        _memoryOwner = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Memory.Length; ++i)
        {
            yield return Memory.Span[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Contains(T item)
    {
        return Memory.Span.Contains(item);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void CopyTo(T[] array, int arrayIndex)
    {
        Memory.Span.CopyTo(array.AsSpan(arrayIndex));
    }

    void ICollection<T>.Add(T item)
    {
        throw new NotSupportedException();
    }

    void ICollection<T>.Clear()
    {
        throw new NotSupportedException();
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new NotSupportedException();
    }
}
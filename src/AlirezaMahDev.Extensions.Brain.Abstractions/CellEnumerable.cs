namespace AlirezaMahDev.Extensions.Brain.Abstractions;

// ReSharper disable once RedundantExtendsListEntry
public readonly struct CellEnumerable<T>(int count, IEnumerable<T> enumerable) : IEnumerable<T>, ICollection<T>
{
    public static CellEnumerable<T> Empty { get; } = new(0, []);

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return count;
        }
    }

    public bool IsReadOnly
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return true;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (count == 0)
        {
            yield break;
        }

        foreach (var item in enumerable)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    [MustDisposeResource]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public CellMemory<T> ToCellMemory()
    {
        return new(this);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        throw new NotSupportedException();
    }

    bool ICollection<T>.Contains(T item)
    {
        throw new NotSupportedException();
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
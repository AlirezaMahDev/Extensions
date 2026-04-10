using System.Diagnostics.CodeAnalysis;

namespace AlirezaMahDev.Extensions.Abstractions;


public interface IRefEnumerator<TRefEnumerator, T>
    where TRefEnumerator : IRefEnumerator<TRefEnumerator, T>, allows ref struct
{
    ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool MoveNext();
}


public interface IRefReadOnlyEnumerator<TRefReadOnlyEnumerator, T>
    where TRefReadOnlyEnumerator : IRefReadOnlyEnumerator<TRefReadOnlyEnumerator, T>, allows ref struct
{
    ref readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    bool MoveNext();
}

public interface IRefEnumerableCore<TRefEnumeratorCore>
    where TRefEnumeratorCore : allows ref struct
{
    TRefEnumeratorCore GetEnumerator();
}

public interface IRefEnumerable<TSelf, T, TRefEnumerator> : IRefEnumerableCore<TRefEnumerator>
    where TSelf : IRefEnumerable<TSelf, T, TRefEnumerator>, allows ref struct
    where TRefEnumerator : IRefEnumerator<TRefEnumerator, T>, allows ref struct;


public interface IRefReadOnlyEnumerable<TSelf, T, TRefReadOnlyEnumerator> : IRefEnumerableCore<TRefReadOnlyEnumerator>
    where TSelf : IRefReadOnlyEnumerable<TSelf, T, TRefReadOnlyEnumerator>, allows ref struct
    where TRefReadOnlyEnumerator : IRefReadOnlyEnumerator<TRefReadOnlyEnumerator, T>, allows ref struct;

public interface IRefCountable<TSelf, T, TRefEnumerator> : IRefEnumerable<TSelf, T, TRefEnumerator>, IRefLength
    where TSelf : IRefCountable<TSelf, T, TRefEnumerator>, allows ref struct
    where TRefEnumerator : IRefEnumerator<TRefEnumerator, T>, allows ref struct;


public interface IRefReadOnlyCountable<TSelf, T, TRefReadOnlyEnumerator> : IRefReadOnlyEnumerable<TSelf, T, TRefReadOnlyEnumerator>, IRefLength
    where TSelf : IRefReadOnlyCountable<TSelf, T, TRefReadOnlyEnumerator>, allows ref struct
    where TRefReadOnlyEnumerator : IRefReadOnlyEnumerator<TRefReadOnlyEnumerator, T>, allows ref struct;


public interface IRefIndexable<TSelf, T> : IRefCountable<TSelf, T, RefIndexableEnumerator<TSelf, T>>
    where TSelf : IRefIndexable<TSelf, T>, allows ref struct
{
    ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}

public interface IRefReadOnlyIndexable<TSelf, T> : IRefReadOnlyCountable<TSelf, T, RefReadOnlyIndexableEnumerator<TSelf, T>>
    where TSelf : IRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}

public ref struct RefIndexableEnumerator<TSelf, T>(TSelf self) : IRefEnumerator<RefIndexableEnumerator<TSelf, T>, T>
    where TSelf : IRefIndexable<TSelf, T>, allows ref struct
{
    private int _index = -1;
    private readonly TSelf _self = self;

    public readonly ref T Current => ref _self[_index];

    public bool MoveNext()
    {
        _index++;
        return _index < _self.Length;
    }
}

public ref struct RefReadOnlyIndexableEnumerator<TSelf, T>(TSelf self) : IRefReadOnlyEnumerator<RefReadOnlyIndexableEnumerator<TSelf, T>, T>
    where TSelf : IRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    private int _index = -1;
    private readonly TSelf _self = self;

    public readonly ref readonly T Current => ref _self[_index];

    public bool MoveNext()
    {
        _index++;
        return _index < _self.Length;
    }
}

public interface IRefBlock<TSelf, T> : IRefIndexable<TSelf, T>
    where TSelf : IRefBlock<TSelf, T>, allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    TSelf Slice(int start, int length);
}

public interface IRefReadOnlyBlock<TSelf, T> : IRefReadOnlyIndexable<TSelf, T>
    where TSelf : IRefReadOnlyBlock<TSelf, T>, allows ref struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    TSelf Slice(int start, int length);
}


public ref struct RefBlock<TSelf, T>(TSelf self) : IRefBlock<RefBlock<TSelf, T>, T>
    where TSelf : IRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _start;
    private int _length;

    public readonly ref T this[int index]
    {
        get
        {
            if (index >= _length)
                throw new IndexOutOfRangeException($"${index} >= {_length}");
            return ref _self[_start + index];
        }
    }

    public readonly int Length => _length;

    public readonly RefIndexableEnumerator<RefBlock<TSelf, T>, T> GetEnumerator()
    {
        return new(this);
    }

    public RefBlock<TSelf, T> Slice(int start, int length)
    {
        if (start + length >= _length)
            throw new IndexOutOfRangeException();
        return this with { _start = _start + start, _length = length };
    }
}


public ref struct RefReadOnlyBlock<TSelf, T>(TSelf self) : IRefReadOnlyBlock<RefReadOnlyBlock<TSelf, T>, T>
    where TSelf : IRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _start;
    private int _length;

    public readonly ref readonly T this[int index]
    {
        get
        {
            if (index >= _length)
                throw new IndexOutOfRangeException($"{index} >= {_length}");
            return ref _self[_start + index];
        }
    }

    public readonly int Length => _length;

    public readonly RefReadOnlyIndexableEnumerator<RefReadOnlyBlock<TSelf, T>, T> GetEnumerator()
    {
        return new(this);
    }

    public RefReadOnlyBlock<TSelf, T> Slice(int start, int length)
    {
        if (start + length >= _length)
            throw new IndexOutOfRangeException($"{start + length} >= {_length}");
        return this with { _start = _start + start, _length = length };
    }
}

public interface IRefList<TSelf, T> : IRefIndexable<TSelf, T>
    where TSelf : IRefList<TSelf, T>, allows ref struct
{
    int Add(in T value);
    int Add(params ReadOnlySpan<T> values);
    bool Insert(in int index, in T value);
    bool Insert(in int index, params ReadOnlySpan<T> values);
    bool Remove(in int index, [NotNullWhen(true)] out T? result);
    bool Remove(in int index, Span<T> result);
    void Clean();
}

public interface IRefDictionary<TSelf, TKey, TValue> : IRefLength
    where TSelf : IRefDictionary<TSelf, TKey, TValue>, allows ref struct
{
    bool TryGet(in TKey key, out TValue value);
    bool TryRemove(in TKey key);
    bool TryAdd(in TKey key, in TValue value);
    bool TryAdd(in TKey key, ScopedRefReadOnlyFunc<TKey, TValue> func);
    TValue GetOrAdd(in TKey key, in TValue value);
    TValue GetOrAdd(in TKey key, ScopedRefReadOnlyFunc<TKey, TValue> func);
    void Clean();
}

public interface IRefLength
{
    int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    }
}

public readonly ref struct RefEnumeratorToRefReadOnlyEnumerator<TRefEnumerator, T>(TRefEnumerator refEnumerator)
    : IRefReadOnlyEnumerator<RefEnumeratorToRefReadOnlyEnumerator<TRefEnumerator, T>, T>
    where TRefEnumerator : IRefEnumerator<TRefEnumerator, T>, allows ref struct
{
    private readonly TRefEnumerator _refEnumerator = refEnumerator;
    public ref readonly T Current => ref _refEnumerator.Current;
    public bool MoveNext() => _refEnumerator.MoveNext();
}

public readonly ref struct RefIndexableItem<TSelf, T>(TSelf self, int index)
    where TSelf : IRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    public int Index { get; } = index;
    public ref T Value => ref _self[Index];
}

public interface IRefStack<TSelf, T> : IRefIndexable<TSelf, T>
    where TSelf : IRefStack<TSelf, T>, allows ref struct
{
    bool TryPop([NotNullWhen(true)] out T? result);
    bool TryPeek([NotNullWhen(true)] out RefIndexableItem<TSelf, T> result);
    bool TryPush(in T value);
    void Clean();
}

public interface IRefQueue<TSelf, T> : IRefIndexable<TSelf, T>
    where TSelf : IRefQueue<TSelf, T>, allows ref struct
{
    bool TryPeek([NotNullWhen(true)] out RefIndexableItem<TSelf, T> result);
    bool TryDequeue([NotNullWhen(true)] out T? result);
    bool TryEnqueue(in T value);
    void Clean();
}

public interface IRefPool<TSelf, T> : IRefIndexable<TSelf, T>
    where TSelf : IRefPool<TSelf, T>, allows ref struct
{
    RefIndexableItem<TSelf, T> Rent();
    void Return(RefIndexableItem<TSelf, T> item);
    void Clean();
}

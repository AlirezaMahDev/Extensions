using System.Diagnostics.CodeAnalysis;
using System.Numerics;

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

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct RefIndexableEnumerator<TSelf, T>(TSelf self) : IRefEnumerator<RefIndexableEnumerator<TSelf, T>, T>
where TSelf : IRefIndexable<TSelf, T>, allows ref struct
{
    private int _index = -1;
    private readonly TSelf _self = self;

    public readonly ref T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _self[_index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        _index++;
        return _index < _self.Length;
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct RefReadOnlyIndexableEnumerator<TSelf, T>(TSelf self) : IRefReadOnlyEnumerator<RefReadOnlyIndexableEnumerator<TSelf, T>, T>
where TSelf : IRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    private int _index = -1;
    private readonly TSelf _self = self;

    public readonly ref readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => ref _self[_index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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


[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct RefBlock<TSelf, T>(TSelf self) : IRefBlock<RefBlock<TSelf, T>, T>
where TSelf : IRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _start;
    private int _length;

    public readonly ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            if (index >= _length)
                throw new IndexOutOfRangeException($"${index} >= {_length}");
            return ref _self[_start + index];
        }
    }

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => _length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly RefIndexableEnumerator<RefBlock<TSelf, T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefBlock<TSelf, T> Slice(int start, int length)
    {
        if (start + length > _length)
            throw new IndexOutOfRangeException($"{start + length} > {_length}");
        return this with { _start = _start + start, _length = length };
    }
}


[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public ref struct RefReadOnlyBlock<TSelf, T>(TSelf self) : IRefReadOnlyBlock<RefReadOnlyBlock<TSelf, T>, T>
where TSelf : IRefReadOnlyIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    private int _start;
    private int _length;

    public readonly ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            if (index >= _length)
                throw new IndexOutOfRangeException($"{index} >= {_length}");
            return ref _self[_start + index];
        }
    }

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly RefReadOnlyIndexableEnumerator<RefReadOnlyBlock<TSelf, T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefReadOnlyBlock<TSelf, T> Slice(int start, int length)
    {
        if (start + length > _length)
            throw new IndexOutOfRangeException($"{start + length} > {_length}");
        return this with { _start = _start + start, _length = length };
    }
}

public interface IRefList<TSelf, T> : IRefIndexable<TSelf, T>
    where TSelf : IRefList<TSelf, T>, allows ref struct
{
    int Add(in T value);
    int Add(params ReadOnlySpan<T> values);
    bool Insert(int index, in T value);
    bool Insert(int index, params ReadOnlySpan<T> values);
    bool Remove(int index, [NotNullWhen(true)] out T? result);
    bool Remove(int index, Span<T> result);
    void Clean();
}

public interface IRefDictionary<TSelf, TKey, TValue> : IRefLength
    where TSelf : IRefDictionary<TSelf, TKey, TValue>, allows ref struct
{
    bool TryGet(in TKey key, out TValue value);
    bool TryRemove(in TKey key, [NotNullWhen(true)] out TValue value);
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
    public ref readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _refEnumerator.Current;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext() => _refEnumerator.MoveNext();
}

public readonly ref struct RefIndexableItem<TSelf, T>(TSelf self, int index)
    where TSelf : IRefIndexable<TSelf, T>, allows ref struct
{
    private readonly TSelf _self = self;
    public int Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
    } = index;
    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _self[Index];
        }
    }
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

public unsafe struct NativeRefList<T> : IRefList<NativeRefList<T>, T>, IDisposable
    where T : unmanaged
{
    private T* _pointer;
    private int _capacity;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeRefList<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    public readonly Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return new(_pointer, Length);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public NativeRefList(int capacity, bool init)
    {
        _capacity = capacity;
        nuint byteCount = (nuint)_capacity * (nuint)Unsafe.SizeOf<T>();
        _pointer = (T*)NativeMemory.Alloc(byteCount);
        if (_pointer == null)
            throw new OutOfMemoryException();
        NativeMemory.Clear(_pointer, byteCount);
        Length = init ? _capacity : 0;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void Grow(int size)
    {
        int lastCapacity = _capacity;
        nuint oldByteCount = (nuint)lastCapacity * (nuint)Unsafe.SizeOf<T>();
        _capacity = (int)BitOperations.RoundUpToPowerOf2((uint)_capacity + (uint)size);
        nuint newByteCount = (nuint)_capacity * (nuint)Unsafe.SizeOf<T>();
        _pointer = (T*)NativeMemory.Realloc(_pointer, newByteCount);
        if (_pointer == null)
        {
            _capacity = lastCapacity;
            throw new OutOfMemoryException();
        }
        NativeMemory.Clear(_pointer + lastCapacity, newByteCount - oldByteCount);
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private set;
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _pointer[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Add(in T value)
    {
        if (Length + 1 > _capacity)
            Grow(1);
        _pointer[Length] = value;
        return Length++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Add(params ReadOnlySpan<T> values)
    {
        if (values.Length == 0)
            throw new Exception("Input count is 0");
        if (Length + values.Length > _capacity)
            Grow(values.Length);
        var span = new Span<T>(_pointer, _capacity);
        values.CopyTo(span[Length..]);
        var length = Length;
        Length += values.Length;
        return length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Insert(int index, in T value)
    {
        if (index < 0 || index > Length)
            throw new Exception("Invalid index");
        if (Length + 1 > _capacity)
            Grow(1);
        var span = new Span<T>(_pointer, _capacity);
        span[index..Length].CopyTo(span[(index + 1)..]);
        span[index] = value;
        Length++;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Insert(int index, params ReadOnlySpan<T> values)
    {
        if (index < 0 || index > Length || values.Length == 0)
            throw new Exception("Invalid index");
        if (Length + values.Length > _capacity)
            Grow(values.Length);
        var span = new Span<T>(_pointer, _capacity);
        span[index..Length].CopyTo(span[(index + values.Length)..]);
        values.CopyTo(span[index..]);
        Length += values.Length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Remove(int index, [NotNullWhen(true)] out T result)
    {
        if (index < 0 || index >= Length)
        {
            result = default;
            throw new Exception("Invalid index");
        }

        var span = new Span<T>(_pointer, _capacity);
        result = span[index];
        span[(index + 1)..].CopyTo(span[index..]);
        Length--;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Remove(int index, Span<T> result)
    {
        if (index < 0 || index + result.Length > Length)
        {
            throw new Exception("Invalid index");
        }

        var span = new Span<T>(_pointer, _capacity);
        span.Slice(index, result.Length).CopyTo(result);
        span[(index + result.Length)..].CopyTo(span[index..]);
        Length -= result.Length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly RefIndexableEnumerator<NativeRefList<T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (_pointer is null)
            return;
        NativeMemory.Free(_pointer);
        _pointer = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        Length = 0;
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct NativeRefStack<T>(int capacity, bool init) : IRefStack<NativeRefStack<T>, T>, IDisposable
where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeRefStack<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    private readonly NativeRefList<T> _list = new(capacity, init);

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _list[index];
        }
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _list.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _list.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefIndexableEnumerator<NativeRefStack<T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPeek([NotNullWhen(true)] out RefIndexableItem<NativeRefStack<T>, T> result)
    {
        if (_list.Length == 0)
        {
            result = default;
            return false;
        }

        result = new(this, _list.Length - 1);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPop([NotNullWhen(true)] out T result)
    {
        if (_list.Length == 0)
        {
            result = default;
            return false;
        }

        _list.Remove(_list.Length - 1, out result);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPush(in T value)
    {
        return _list.Add(in value) != -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        _list.Clean();
    }
}

[method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
public readonly struct NativeRefQueue<T>(int capacity = 1, bool init = false) : IRefQueue<NativeRefQueue<T>, T>, IDisposable
where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeRefQueue<T> Create(int capacity = 1, bool init = false) => new(capacity, init);
    private readonly NativeRefList<T> _list = new(capacity, init);

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _list.Length;
        }
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return ref _list[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        _list.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryDequeue([NotNullWhen(true)] out T result)
    {
        if (_list.Length == 0)
        {
            result = default;
            return false;
        }

        _list.Remove(0, out result);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryEnqueue(in T value)
    {
        return _list.Add(in value) != -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool TryPeek([NotNullWhen(true)] out RefIndexableItem<NativeRefQueue<T>, T> result)
    {
        if (_list.Length == 0)
        {
            result = default;
            return false;
        }

        result = new(this, 0);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RefIndexableEnumerator<NativeRefQueue<T>, T> GetEnumerator()
    {
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Clean()
    {
        _list.Clean();
    }
}

public static class RefIndexableExtensions
{
    extension<TSelf, T>(TSelf self)
        where TSelf : IRefIndexable<TSelf, T>, allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public RefBlock<TSelf, T> AsRefBlock()
        {
            return new(self);
        }
    }
}
public static class RefReadOnlyIndexableExtensions
{
    extension<TSelf, T>(TSelf self)
        where TSelf : IRefReadOnlyIndexable<TSelf, T>, allows ref struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public RefReadOnlyBlock<TSelf, T> AsRefReadOnlyBlock()
        {
            return new(self);
        }
    }
}

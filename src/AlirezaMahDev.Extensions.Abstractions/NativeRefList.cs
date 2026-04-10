using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace AlirezaMahDev.Extensions.Abstractions;

public unsafe struct NativeRefList<T> : IRefList<NativeRefList<T>, T>, IDisposable
    where T : unmanaged
{
    private T* _pointer;
    private int _capacity;

    public static NativeRefList<T> Create(int capacity = 1) => new(capacity);

    public NativeRefList(int capacity)
    {
        _capacity = capacity;
        nuint byteCount = (nuint)_capacity * (nuint)Unsafe.SizeOf<T>();
        _pointer = (T*)NativeMemory.Alloc(byteCount);
        if (_pointer == null)
            throw new OutOfMemoryException();
        NativeMemory.Clear(_pointer, byteCount);
        Length = 0;
    }
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

    public int Length { get; private set; }

    public ref T this[int index] => ref _pointer[index];

    public int Add(in T value)
    {
        if (Length + 1 > _capacity)
            Grow(1);
        _pointer[Length] = value;
        return Length++;
    }

    public int Add(params ReadOnlySpan<T> values)
    {
        if (values.Length == 0)
            return -1;
        if (Length + values.Length > _capacity)
            Grow(values.Length);
        var span = new Span<T>(_pointer, _capacity);
        values.CopyTo(span[Length..]);
        var length = Length;
        Length += values.Length;
        return length;
    }

    public bool Insert(in int index, in T value)
    {
        if (index < 0 || index > Length)
            return false;
        if (Length + 1 > _capacity)
            Grow(1);
        var span = new Span<T>(_pointer, _capacity);
        span[index..Length].CopyTo(span[(index + 1)..]);
        span[index] = value;
        Length++;
        return true;
    }

    public bool Insert(in int index, params ReadOnlySpan<T> values)
    {
        if (index < 0 || index > Length || values.Length == 0)
            return false;
        if (Length + values.Length > _capacity)
            Grow(values.Length);
        var span = new Span<T>(_pointer, _capacity);
        span[index..Length].CopyTo(span[(index + values.Length)..]);
        values.CopyTo(span[index..]);
        Length += values.Length;
        return true;
    }

    public bool Remove(in int index, [NotNullWhen(true)] out T result)
    {
        if (index < 0 || index >= Length)
        {
            result = default;
            return false;
        }

        var span = new Span<T>(_pointer, _capacity);
        result = span[index];
        span[(index + 1)..].CopyTo(span[index..]);
        Length--;
        return true;
    }

    public bool Remove(in int index, Span<T> result)
    {
        if (index < 0 || index + result.Length > Length)
        {
            return false;
        }

        var span = new Span<T>(_pointer, _capacity);
        span.Slice(index, result.Length).CopyTo(result);
        span[(index + result.Length)..].CopyTo(span[index..]);
        Length -= result.Length;
        return true;
    }

    public readonly RefIndexableEnumerator<NativeRefList<T>, T> GetEnumerator()
    {
        return new(this);
    }

    public void Dispose()
    {
        if (_pointer is null)
            return;
        NativeMemory.Free(_pointer);
        _pointer = null;
    }

    public void Clean()
    {
        Length = 0;
    }
}

public readonly struct NativeRefStack<T>(int capacity) : IRefStack<NativeRefStack<T>, T>, IDisposable
    where T : unmanaged
{
    public static NativeRefStack<T> Create(int capacity = 1) => new(capacity);

    private readonly NativeRefList<T> _list = new(capacity);

    public ref T this[int index] => ref _list[index];

    public int Length => _list.Length;

    public void Dispose()
    {
        _list.Dispose();
    }

    public RefIndexableEnumerator<NativeRefStack<T>, T> GetEnumerator()
    {
        return new(this);
    }

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

    public bool TryPush(in T value)
    {
        return _list.Add(in value) != -1;
    }

    public void Clean()
    {
        _list.Clean();
    }
}

public readonly struct NativeRefQueue<T>(int capacity = 1) : IRefQueue<NativeRefQueue<T>, T>, IDisposable
    where T : unmanaged
{
    public static NativeRefQueue<T> Create(int capacity = 1) => new(capacity);
    private readonly NativeRefList<T> _list = new(capacity);

    public int Length => _list.Length;

    public ref T this[int index] => ref _list[index];

    public void Dispose()
    {
        _list.Dispose();
    }

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

    public bool TryEnqueue(in T value)
    {
        return _list.Add(in value) != -1;
    }

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

    public RefIndexableEnumerator<NativeRefQueue<T>, T> GetEnumerator()
    {
        return new(this);
    }

    public void Clean()
    {
        _list.Clean();
    }
}

public struct ConcurrencyNativeRefList<T>(int capacity) : IRefList<ConcurrencyNativeRefList<T>, T>, IDisposable
    where T : unmanaged
{
    public static ConcurrencyNativeRefList<T> Create(int capacity = 1) => new(capacity);
    private ReaderWriterLocker _lock = new();
    private NativeRefList<T> _list = new(capacity);
    public void Dispose()
    {
        _list.Dispose();
    }

    public ref T this[int index]
    {
        get
        {
            using var @lock = _lock.EnterReadLockScope();
            return ref _list[index];
        }
    }

    public int Length
    {
        get
        {
            using var @lock = _lock.EnterReadLockScope();
            return _list.Length;
        }
    }

    public int Add(in T value)
    {
        using var @lock = _lock.EnterWriteLockScope();
        return _list.Add(in value);
    }

    public int Add(params ReadOnlySpan<T> values)
    {
        using var @lock = _lock.EnterWriteLockScope();
        return _list.Add(values);
    }

    public bool Insert(in int index, in T value)
    {
        using var @lock = _lock.EnterWriteLockScope();
        return _list.Insert(in index, in value);
    }

    public bool Insert(in int index, params ReadOnlySpan<T> values)
    {
        using var @lock = _lock.EnterWriteLockScope();
        return _list.Insert(in index, values);
    }

    public bool Remove(in int index, [NotNullWhen(true)] out T result)
    {
        using var @lock = _lock.EnterWriteLockScope();
        return _list.Remove(in index, out result);
    }

    public bool Remove(in int index, Span<T> result)
    {
        using var @lock = _lock.EnterWriteLockScope();
        return _list.Remove(in index, result);
    }
    public readonly RefIndexableEnumerator<ConcurrencyNativeRefList<T>, T> GetEnumerator()
    {
        return new(this);
    }

    public void Clean()
    {
        _list.Clean();
    }
}

public struct ConcurrencyNativeRefStack<T>(int capacity) : IRefStack<ConcurrencyNativeRefStack<T>, T>, IDisposable
    where T : unmanaged
{
    public static ConcurrencyNativeRefStack<T> Create(int capacity = 1) => new(capacity);

    private ReaderWriterLocker _lock = new();
    private readonly NativeRefStack<T> _stack = new(capacity);

    public int Length
    {
        get
        {
            using var @lock = _lock.EnterReadLockScope();
            return _stack.Length;
        }
    }

    public ref T this[int index]
    {
        get
        {
            using var @lock = _lock.EnterReadLockScope();
            return ref _stack[index];
        }
    }

    public readonly void Dispose()
    {
        _stack.Dispose();
    }

    public bool TryPop([NotNullWhen(true)] out T result)
    {
        using var @lock = _lock.EnterWriteLockScope();
        return _stack.TryPop(out result);
    }

    public bool TryPeek([NotNullWhen(true)] out RefIndexableItem<ConcurrencyNativeRefStack<T>, T> result)
    {
        using var @lock = _lock.EnterReadLockScope();
        if (_stack.TryPeek(out var refIndexableItem))
        {
            result = new(this, refIndexableItem.Index);
            return true;
        }
        result = default;
        return false;
    }

    public bool TryPush(in T value)
    {
        using var @lock = _lock.EnterWriteLockScope();
        return _stack.TryPush(value);
    }

    public readonly RefIndexableEnumerator<ConcurrencyNativeRefStack<T>, T> GetEnumerator()
    {
        return new(this);
    }

    public readonly void Clean()
    {
        _stack.Clean();
    }
}

public struct ConcurrencyNativeRefQueue<T>(int capacity) : IRefQueue<ConcurrencyNativeRefQueue<T>, T>, IDisposable
    where T : unmanaged
{
    public static ConcurrencyNativeRefQueue<T> Create(int capacity = 1) => new(capacity);

    private ReaderWriterLocker _lock = new();
    private readonly NativeRefQueue<T> _queue = new(capacity);

    public readonly void Dispose()
    {
        _queue.Dispose();
    }

    public int Length
    {
        get
        {
            using var @lock = _lock.EnterReadLockScope();
            return _queue.Length;
        }
    }

    public ref T this[int index]
    {
        get
        {
            using var @lock = _lock.EnterReadLockScope();
            return ref _queue[index];
        }
    }

    public bool TryPeek([NotNullWhen(true)] out RefIndexableItem<ConcurrencyNativeRefQueue<T>, T> result)
    {
        using var @lock = _lock.EnterReadLockScope();
        if (_queue.TryPeek(out var item))
        {
            result = new(this, item.Index);
            return true;
        }

        result = default;
        return false;
    }

    public bool TryDequeue([NotNullWhen(true)] out T result)
    {
        using var @lock = _lock.EnterWriteLockScope();
        return _queue.TryDequeue(out result);
    }

    public bool TryEnqueue(in T value)
    {
        using var @lock = _lock.EnterWriteLockScope();
        return _queue.TryEnqueue(value);
    }

    public readonly RefIndexableEnumerator<ConcurrencyNativeRefQueue<T>, T> GetEnumerator()
    {
        return new(this);
    }

    public readonly void Clean()
    {
        _queue.Clean();
    }
}

public readonly struct ConcurrencyNativeRefPool<T>(int capacity) : IRefPool<ConcurrencyNativeRefPool<T>, T>, IDisposable
    where T : unmanaged
{
    public static ConcurrencyNativeRefList<T> Create(int capacity = 1) => new(capacity);

    private readonly ConcurrencyNativeRefStack<int> _free = new(capacity);
    private readonly ConcurrencyNativeRefList<T> _used = new(capacity);

    public ref T this[int index] => ref _used[index];

    public readonly int Length => _used.Length;

    public void Clean()
    {
        _free.Clean();
    }

    public void Dispose()
    {
        _free.Dispose();
        _used.Dispose();
    }

    public RefIndexableItem<ConcurrencyNativeRefPool<T>, T> Rent()
    {
        return _free.TryPop(out var index)
            ? new(this, index)
            : new(this, _used.Add(default));
    }

    public void Return(RefIndexableItem<ConcurrencyNativeRefPool<T>, T> item)
    {
        _free.TryPush(item.Index);
    }

    public RefIndexableEnumerator<ConcurrencyNativeRefPool<T>, T> GetEnumerator()
    {
        return new(this);
    }
}

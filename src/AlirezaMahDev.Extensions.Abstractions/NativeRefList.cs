using System.Numerics;

namespace AlirezaMahDev.Extensions.Abstractions;

[CollectionBuilder(typeof(NativeRefListCollectionBuilder), nameof(NativeRefListCollectionBuilder.Create))]
public unsafe struct NativeRefList<T> : IRefList<NativeRefList<T>, T>, IDisposable
    where T : unmanaged
{
    private T* _pointer;
    private int _capacity;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeRefList<T> Create(int capacity = 1, bool init = false) => new(capacity, init);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static NativeRefList<T> Create(params ReadOnlySpan<T> values)
    {
        var list = NativeRefList<T>.Create(values.Length, true);
        values.CopyTo(list.Span);
        return list;
    }

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
            return -1;
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
            return false;
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
            return false;
        if (Length + values.Length > _capacity)
            Grow(values.Length);
        var span = new Span<T>(_pointer, _capacity);
        span[index..Length].CopyTo(span[(index + values.Length)..]);
        values.CopyTo(span[index..]);
        Length += values.Length;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Remove(int index, out T result)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Remove(int index, Span<T> result)
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
using System.IO.Hashing;

namespace AlirezaMahDev.Extensions.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
[MustDisposeResource]
public readonly ref struct XxHash3Builder() : IDisposable
{
    private readonly XxHash3 _hasher = XxHash3Pool.Rent();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Add<T>(in T value)
        where T : unmanaged
    {
        _hasher.Append(MemoryMarshal.AsBytes(
            MemoryMarshal.CreateReadOnlySpan(in value, 1)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void AddSpan<T>(ReadOnlySpan<T> values)
        where T : unmanaged
    {
        _hasher.Append(MemoryMarshal.AsBytes(values));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int ToHashCode()
    {
        return (int)_hasher.GetCurrentHashAsUInt64();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        XxHash3Pool.Return(_hasher);
    }
}
using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly struct DataLocation(IDataAccess access, long offset, Memory<byte> memory) : IDataLocation<DataLocation>
{
    public long Offset => offset;
    public IDataAccess Access => access;

    public Memory<byte> Memory => memory;
    public int Length => Memory.Length;

    public static DataLocation Create(IDataAccess access, int length)
    {
        var allocateMemory = access.AllocateMemory(length);
        return new(access, allocateMemory.Offset, allocateMemory.Memory);
    }

    public static async ValueTask<DataLocation> CreateAsync(IDataAccess access,
        int length,
        CancellationToken cancellationToken = default)
    {
        var allocateMemory = await access.AllocateMemoryAsync(length, cancellationToken);
        return new(access, allocateMemory.Offset, allocateMemory.Memory);
    }

    public static DataLocation Read(IDataAccess access, long offset, int length) =>
        new(access, offset, access.ReadMemory(offset, length));

    public static async ValueTask<DataLocation> ReadAsync(IDataAccess access,
        long offset,
        int length,
        CancellationToken cancellationToken) =>
        new(access, offset, await access.ReadMemoryAsync(offset, length, cancellationToken));

    public bool Equals(DataLocation other)
    {
        return Offset == other.Offset && Length == other.Length;
    }

    public override bool Equals(object? obj)
    {
        return obj is DataLocation location && Equals(location);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Offset, Length);
    }

    public static bool operator ==(DataLocation left, DataLocation right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DataLocation left, DataLocation right)
    {
        return !(left == right);
    }
}

public readonly struct DataLocation<TValue>(DataLocation @base) : IDataLocation<DataLocation<TValue>, TValue>
    where TValue : unmanaged, IDataValue<TValue>
{
    public DataLocation Base => @base;

    public long Offset => Base.Offset;
    public int Length => Base.Length;

    public IDataAccess Access => Base.Access;
    public Memory<byte> Memory => Base.Memory;

    public ref TValue RefValue =>
        ref MemoryMarshal.AsRef<TValue>(Base.Memory.Span);

    public static DataLocation<TValue> Create(IDataAccess access, TValue @default)
    {
        var location = new DataLocation<TValue>(DataLocation.Create(access, TValue.ValueSize));
        if (location.RefValue.Equals(default))
            location.RefValue = @default;
        return location;
    }

    public static async ValueTask<DataLocation<TValue>> CreateAsync(IDataAccess access,
        TValue @default,
        CancellationToken cancellationToken = default)
    {
        var location =
            new DataLocation<TValue>(await DataLocation.CreateAsync(access, TValue.ValueSize, cancellationToken));
        if (location.RefValue.Equals(default))
            location.RefValue = @default;
        return location;
    }

    public static DataLocation<TValue> Read(IDataAccess access, long offset)
    {
        return new(DataLocation.Read(access, offset, TValue.ValueSize));
    }

    public static async ValueTask<DataLocation<TValue>> ReadAsync(IDataAccess access,
        long offset,
        CancellationToken cancellationToken)
    {
        return new(await DataLocation.ReadAsync(access, offset, TValue.ValueSize, cancellationToken));
    }

    public bool Equals(DataLocation<TValue> other)
    {
        return Base.Equals(other.Base);
    }

    public override bool Equals(object? obj)
    {
        return obj is DataLocation<TValue> location && Equals(location);
    }

    public override int GetHashCode()
    {
        return Base.GetHashCode();
    }

    public static bool operator ==(DataLocation<TValue> left, DataLocation<TValue> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DataLocation<TValue> left, DataLocation<TValue> right)
    {
        return !(left == right);
    }
}
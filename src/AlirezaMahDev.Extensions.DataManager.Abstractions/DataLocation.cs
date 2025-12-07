using System.Runtime.InteropServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly struct DataLocation(IDataAccess access, long offset, Memory<byte> memory) : IDataLocation<DataLocation>
{
    public long Offset => offset;
    public int Length => memory.Length;

    public IDataAccess Access => access;
    public Memory<byte> Memory => memory;

    public static DataLocation Create(IDataAccess access, int length)
    {
        return Read(access, access.AllocateOffset(length), length);
    }

    public static async ValueTask<DataLocation> CreateAsync(IDataAccess access,
        int length,
        CancellationToken cancellationToken = default)
    {
        return await ReadAsync(access, access.AllocateOffset(length), length, cancellationToken);
    }

    public static DataLocation Read(IDataAccess access, long offset, int length) =>
        new(access, offset, access.ReadMemory(offset, length));

    public static async ValueTask<DataLocation> ReadAsync(IDataAccess access,
        long offset,
        int length,
        CancellationToken cancellationToken) =>
        new(access, offset, await access.ReadMemoryAsync(offset, length, cancellationToken));


    public static void Write(IDataAccess access, DataLocation location)
    {
        access.WriteMemory(location.Offset, location.Memory);
    }

    public static async ValueTask WriteAsync(IDataAccess access,
        DataLocation location,
        CancellationToken cancellationToken = default)
    {
        await access.WriteMemoryAsync(location.Offset, location.Memory, cancellationToken);
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

    public static void Write(IDataAccess access, DataLocation<TValue> location)
    {
        DataLocation.Write(access, location.Base);
    }

    public static async ValueTask WriteAsync(IDataAccess access,
        DataLocation<TValue> location,
        CancellationToken cancellationToken = default)
    {
        await DataLocation.WriteAsync(access, location.Base, cancellationToken);
    }
}

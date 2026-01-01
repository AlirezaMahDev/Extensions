namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly record struct DataLocation(long Offset, int Length) : IDataLocation<DataLocation>
{
    public static DataLocation Create(IDataAccess access, int length) =>
        new(access.AllocateMemory(length).Offset, length);
}

public readonly record struct DataLocation<TValue>(long Offset) : IDataLocation<DataLocation<TValue>, TValue>
    where TValue : unmanaged, IDataValue<TValue>
{
    public int Length => TValue.ValueSize;

    public static DataLocation<TValue> Create(IDataAccess access, TValue @default)
    {
        var location = new DataLocation<TValue>(access.AllocateMemory(TValue.ValueSize).Offset);
        location.GetRefValue(access) = @default;
        return location;
    }
}
namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly record struct DataLocation(DataOffset Offset) : IDataLocation<DataLocation>
{
    public static DataLocation Create(IDataAccess access, int length) =>
        new(access.AllocateMemory(length).Offset);
}

public readonly record struct DataLocation<TValue>(DataOffset Offset) : IDataLocation<DataLocation<TValue>, TValue>
    where TValue : unmanaged, IDataValue<TValue>
{
    public static DataLocation<TValue> Create(IDataAccess access, TValue @default)
    {
        var location = new DataLocation<TValue>(access.AllocateMemory(TValue.ValueSize).Offset);
        location.GetRefValue(access) = @default;
        return location;
    }
}
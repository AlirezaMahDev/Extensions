namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public readonly struct DataLocation<TValue>(IDataLocation location) : IDataBlockAccessorSave
    where TValue : unmanaged
{
    public IDataLocation Location { get; } = location;

    public DataBlockMemoryRefValue<TValue> BlockMemoryRefValue { get; }
        = location.Data.AsMemory().AsRefValue<TValue>();

    public ref String64 RefKey => ref Location.RefKey;
    public ref TValue RefValue => ref BlockMemoryRefValue.RefValue;

    public void Save()
    {
        Location.Save();
    }
}
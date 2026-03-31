namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataValue<TValue> : IScopedRefReadOnlyEquatable<TValue>
    where TValue : unmanaged, IDataValue<TValue>
{
    ref DataLock Lock { get; }
    static virtual int ValueSize { get; } = Unsafe.SizeOf<TValue>();
}
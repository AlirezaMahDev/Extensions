namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataValue<TValue> : IInEquatable<TValue>
    where TValue : unmanaged, IDataValue<TValue>
{
    static virtual int ValueSize { get; } = Unsafe.SizeOf<TValue>();
}
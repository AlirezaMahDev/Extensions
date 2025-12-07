using System.Runtime.CompilerServices;

namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataValue<TValue> : IEquatable<TValue>
    where TValue : unmanaged, IDataValue<TValue>
{
    static virtual int ValueSize { get; } = Unsafe.SizeOf<TValue>();
}
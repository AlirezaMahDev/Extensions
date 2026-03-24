namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataValueDefault<TValue> : IEquatable<TValue>
    where TValue : unmanaged, IDataValue<TValue>
{
    static abstract ref readonly TValue Default { get; }
}
namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataValue<TValue> : IEquatable<TValue>
    where TValue : unmanaged, IDataValue<TValue>;
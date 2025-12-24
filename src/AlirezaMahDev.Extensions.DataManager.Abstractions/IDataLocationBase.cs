namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataLocationBase<TSelf> : IEquatable<TSelf>
    where TSelf : IDataLocationBase<TSelf>
{
    long Offset { get; }
    int Length { get; }
    IDataAccess Access { get; }
    Memory<byte> Memory { get; }

    static abstract void Write(IDataAccess access, TSelf location);

    static abstract ValueTask WriteAsync(IDataAccess access,
        TSelf location,
        CancellationToken cancellationToken = default);
}

public interface IDataLocation<TSelf, TValue> : IDataLocationBase<TSelf>
    where TSelf : IDataLocation<TSelf, TValue>
    where TValue : unmanaged, IDataValue<TValue>
{
    ref TValue RefValue { get; }
    
    static abstract TSelf Create(IDataAccess access, TValue @default);
    static abstract ValueTask<TSelf> CreateAsync(IDataAccess access,
        TValue @default,
        CancellationToken cancellationToken = default);

    static abstract TSelf Read(IDataAccess access, long offset);

    static abstract ValueTask<TSelf> ReadAsync(IDataAccess access,
        long offset,
        CancellationToken cancellationToken = default);
}

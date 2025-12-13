namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataLocation<TSelf> : IDataLocationBase<TSelf>
    where TSelf : IDataLocation<TSelf>
{
    static abstract TSelf Create(IDataAccess access, int length);

    static abstract ValueTask<TSelf> CreateAsync(IDataAccess access,
        int length,
        CancellationToken cancellationToken = default);

    static abstract TSelf Read(IDataAccess access, long offset, int length);

    static abstract ValueTask<TSelf> ReadAsync(IDataAccess access,
        long offset,
        int length,
        CancellationToken cancellationToken = default);
}
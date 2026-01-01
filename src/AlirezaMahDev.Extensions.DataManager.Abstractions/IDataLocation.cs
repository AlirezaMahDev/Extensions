namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataLocation<TSelf> : IDataLocationBase<TSelf>
    where TSelf : IDataLocation<TSelf>
{
    static abstract TSelf Create(IDataAccess access, int length);
}

public interface IDataLocation<TSelf, in TValue> : IDataLocationBase<TSelf>
    where TSelf : IDataLocation<TSelf, TValue>
    where TValue : unmanaged, IDataValue<TValue>
{
    static abstract TSelf Create(IDataAccess access, TValue @default);
}
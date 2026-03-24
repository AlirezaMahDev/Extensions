namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataLocation<TSelf> : IDataLocationBase<TSelf>
    where TSelf : IDataLocation<TSelf>, allows ref struct
{
    static abstract void Create(IDataAccess access, int length, out TSelf result);
}

public interface IDataLocation<TSelf, in TValue> : IDataLocationBase<TSelf>
    where TSelf : IDataLocation<TSelf, TValue>, allows ref struct
    where TValue : unmanaged, IDataValue<TValue>
{
    static abstract void Create(IDataAccess access, TValue @default, out TSelf result);
}
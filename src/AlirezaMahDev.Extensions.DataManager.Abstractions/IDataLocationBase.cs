namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataLocationBase<TSelf> : IInEquatable<TSelf>
    where TSelf : IDataLocationBase<TSelf>, allows ref struct
{
    ref readonly DataOffset Offset { get; }
    Memory<byte> GetMemory(IDataAccess access);
    ref byte GetRef(IDataAccess access);
}
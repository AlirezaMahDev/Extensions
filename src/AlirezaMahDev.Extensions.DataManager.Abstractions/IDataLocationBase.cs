namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataLocationBase<TSelf> : IEquatable<TSelf>
    where TSelf : IDataLocationBase<TSelf>
{
    DataOffset Offset { get; }
}
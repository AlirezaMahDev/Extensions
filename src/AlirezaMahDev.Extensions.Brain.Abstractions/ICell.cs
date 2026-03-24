namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public interface ICell
{
    ref readonly DataOffset Offset { get; }
}
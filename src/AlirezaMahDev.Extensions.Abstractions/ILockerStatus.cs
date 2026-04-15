namespace AlirezaMahDev.Extensions.Abstractions;

public interface ILockerStatus
{
    ReaderWriterLockerState LockerState { get; }
}
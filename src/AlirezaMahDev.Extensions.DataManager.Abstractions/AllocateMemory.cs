namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public record struct AllocateMemory<T>(DataOffset Offset, Memory<T> Memory);

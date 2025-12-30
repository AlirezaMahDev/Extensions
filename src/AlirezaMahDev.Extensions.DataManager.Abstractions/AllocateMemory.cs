namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public record struct AllocateMemory<T>(long Offset, Memory<T> Memory);

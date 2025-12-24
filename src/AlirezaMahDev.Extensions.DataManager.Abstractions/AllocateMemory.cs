namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public readonly record struct AllocateMemory(long Offset, Memory<byte> Memory);
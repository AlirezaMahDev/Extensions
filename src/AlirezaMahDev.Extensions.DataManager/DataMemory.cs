using System.Buffers;
using System.IO.Hashing;

namespace AlirezaMahDev.Extensions.DataManager;

class DataMemory : IMemoryOwner<byte>
{
    private readonly IMemoryOwner<byte> _memoryOwner;
    private UInt128 _hash;

    public DataMemory(int length)
    {
        _memoryOwner = MemoryPool<byte>.Shared.Rent(length);
        Memory = _memoryOwner.Memory[..length];
        Memory.Span.Clear();
    }

    public bool CheckHash =>
        _hash == GenerateHash();

    public void CreateHash() =>
        _hash = GenerateHash();

    private UInt128 GenerateHash() =>
        XxHash128.HashToUInt128(Memory.Span);

    public Memory<byte> Memory { get; }

    public void Dispose() =>
        _memoryOwner.Dispose();
}
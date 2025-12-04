namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public interface IDataBlockAccessor : IDataBlockAccessorSave
{
    DataAddress Address { get; }
    Memory<byte> Memory { get; }
}
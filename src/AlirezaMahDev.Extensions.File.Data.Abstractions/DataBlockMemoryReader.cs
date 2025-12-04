namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public class DataBlockMemoryReader(DataBlockMemory memory)
    : IDataBlockReader<DataBlockMemoryReader>
{
    public DataBlockMemory Memory { get; private set; } = memory;

    public DataBlockMemoryReader Read(int length, out DataBlockMemory result)
    {
        var array = Memory;
        result = array[..length];
        Memory = array[length..];
        return this;
    }

    public DataBlockMemoryReader ReadBlock(out DataBlockMemory result)
    {
        result = Memory;
        return this;
    }
}
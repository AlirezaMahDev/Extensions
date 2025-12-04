namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public class DataBlockReader(IDataBlock access, long id)
    : IDataBlockReader<DataBlockReader>
{
    private long _id = id;

    public DataBlockReader Read(int length, out IDataBlockAccessor result)
    {
        result = access.Read(_id, length);
        _id += length;
        return this;
    }

    public DataBlockReader Read(int length, out DataBlockMemory result)
    {
        Read(length, out IDataBlockAccessor blockAccess);
        result = blockAccess.AsMemory();
        return this;
    }
}
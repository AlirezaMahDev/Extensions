namespace AlirezaMahDev.Extensions.File.Data.Abstractions;

public interface IDataLocation : IDataBlockAccessorSave,
    IDataDictionary<String64, int, IDataLocation>
{
    IDataAccess DataAccess { get; }

    long Id { get; }

    ref String64 RefKey { get; }
    ref DataLocationValue RefValue { get; }

    IDataBlockAccessor Data { get; }

    IDataLocation? Child { get; }
    IDataLocation? Next { get; }

    string ToString();

    IDataLocation ForceAdd(String64 key, int dataLength);
    IDataLocation ForceAdd(IDataLocation location);
}
using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

public class DataStatus : IDataStatus
{
    private readonly DataMap _dataMap;

    internal DataStatus(DataMap dataMap)
    {
        _dataMap = dataMap;
        Files = new (DataFileStatus fileStatus, DataFilePartStatus[] parts)[DataDefaults.FileCount];
        for (var fileId = 0; fileId < DataDefaults.FileCount; fileId++)
        {
            Files[fileId] = (DataFileStatus.NotInitialized, new DataFilePartStatus[DataDefaults.PartCount]);
        }
    }

    public (DataFileStatus fileStatus, DataFilePartStatus[] parts)[] Files { get; }

    public void Refresh()
    {
        for (var fileId = 0; fileId < DataDefaults.FileCount; fileId++)
        {
            ref var fileItem = ref Files[fileId];
            var file = _dataMap.Files.Span[fileId];

            if (!file.IsValueCreated)
            {
                fileItem.fileStatus = DataFileStatus.NotInitialized;
            }
            else
            {
                fileItem.fileStatus = DataFileStatus.Initialized;
                for (var partId = 0; partId < DataDefaults.PartCount; partId++)
                {
                    var part = file.Value.Parts.Span[partId];
                    fileItem.parts[partId] = !part.IsValueCreated
                        ? DataFilePartStatus.NotInitialized
                        : part.Value.HasCache
                            ? DataFilePartStatus.Cached
                            : DataFilePartStatus.Idle;
                }
            }
        }
    }
}
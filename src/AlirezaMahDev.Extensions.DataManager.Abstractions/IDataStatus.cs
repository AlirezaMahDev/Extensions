namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataStatus
{
    (DataFileStatus fileStatus, DataFilePartStatus[] parts)[] Files { get; }
    void Refresh();
}

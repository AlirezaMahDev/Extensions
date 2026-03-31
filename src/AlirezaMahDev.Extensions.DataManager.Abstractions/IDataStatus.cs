namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataStatus
{
    (DataFileStatus fileStatus, (DataFilePartStatus filePartStatus, long count)[] parts)[] Files { get; }
    void Refresh();
}

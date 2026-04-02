namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public interface IDataAccess
{
    string Path { get; }

    IDataMap Map { get; }
    ref readonly DataLocation<DataPath> Root { get; }
    ref readonly DataLocation<DataTrash> Trash { get; }

    DataOffset AllocateOffset(int length);
    IDataMapFilePartOwner GetOwner(in DataOffset offset);

    void Flush();
}
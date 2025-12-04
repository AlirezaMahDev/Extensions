using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public interface ITableData
{
    ITableRow Row { get; }
    int Index { get; }
    string Key { get; }
    DataBlockMemory BlockMemory { get; }
}
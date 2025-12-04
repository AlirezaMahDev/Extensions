using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public interface ITableAccess : IDataLocationItem
{
    ITableColumns Columns { get; }
    ITableRows Rows { get; }
}
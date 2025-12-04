using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public interface ITableRows : IDataLocationItem<TableRowsValue>, IDataCollection<ITableRow>
{
    ITableAccess Table { get; }
}
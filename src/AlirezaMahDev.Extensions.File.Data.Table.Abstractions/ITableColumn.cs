using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public interface ITableColumn : IDataLocationItem<TableColumnValue>
{
    ITableColumns Columns { get; }
    IStackAccess Stack { get; }
    int Size { get; set; }
}
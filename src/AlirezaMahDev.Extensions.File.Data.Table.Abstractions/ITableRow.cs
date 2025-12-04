using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public interface ITableRow : IDataDictionary<String64, ITableData>
{
    ITableRows Rows { get; }
    int Index { get; }
    ref TableRowValue Value { get; }
}
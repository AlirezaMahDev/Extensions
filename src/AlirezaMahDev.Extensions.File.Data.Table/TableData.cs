using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableData(TableDataArgs args) : ITableData
{
    public ITableRow Row { get; } = args.Row;
    public int Index { get; } = args.Index;
    public string Key { get; } = args.Key;

    public DataBlockMemory BlockMemory { get; } =
        args.Row.Rows.Table.Columns.GetOrAdd(args.Key).Stack.Items.GetOrAdd(args.Index).Data;
}
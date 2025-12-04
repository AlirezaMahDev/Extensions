using System.Collections;
using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableRow : ITableRow
{
    public TableRow(TableRowArgs args)
    {
        Rows = args.Rows;
        ItemValue = args.Rows.Stack.Items.GetOrAdd(args.Index).As<TableRowValue>();
        Index = ItemValue.Item.Index;
    }

    private StackItem<TableRowValue> ItemValue { get; }
    public int Index { get; }

    public ref TableRowValue Value => ref ItemValue.RefValue;
    public ITableRows Rows { get; }

    public bool TryGet(String64 key, [MaybeNullWhen(false)] out ITableData value)
    {
        throw new NotImplementedException();
    }

    public ITableData GetOrAdd(String64 key)
    {
        throw new NotImplementedException();
    }

    public bool TryRemove(String64 key, [MaybeNullWhen(false)] out ITableData item)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<ITableData> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
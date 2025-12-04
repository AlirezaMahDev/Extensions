using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Stack;
using AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableColumn : ITableColumn
{
    public TableColumn(TableColumnArgs args)
    {
        Columns = args.Columns;

        LocationValue = args.Columns.Location.GetOrAdd<TableColumnValue>(args.Key);
        Stack = LocationValue.Location.AsStack();
    }

    public DataLocation<TableColumnValue> LocationValue { get; }
    public ITableColumns Columns { get; }
    public IStackAccess Stack { get; }

    public ref String64 RefKey => ref LocationValue.RefKey;
    public ref TableColumnValue RefValue => ref LocationValue.RefValue;
    public int Size { get => Stack.Size; set => Stack.Size = value; }

    public void Save()
    {
        LocationValue.Save();
        Stack.Save();
    }
}
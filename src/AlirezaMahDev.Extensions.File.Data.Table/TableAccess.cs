using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableAccess : ITableAccess
{
    public TableAccess(
        IDataLocation dataLocation,
        TableColumnsFactory tableColumnsFactory,
        TableRowsFactory tableRowsFactory)
    {
        Location = dataLocation.GetOrAdd(".table");

        Columns = tableColumnsFactory.GetOrCreate(this);
        Rows = tableRowsFactory.GetOrCreate(this);
    }

    public ref String64 RefKey => ref Location.RefKey;
    public IDataLocation Location { get; }

    public ITableColumns Columns { get; }
    public ITableRows Rows { get; }

    public void Save()
    {
        Location.Save();
        Columns.Save();
        Rows.Save();
    }
}
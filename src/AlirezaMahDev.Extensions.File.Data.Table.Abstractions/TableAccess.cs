using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public readonly struct TableAccess<TEntity>(ITableAccess table) : IDataBlockAccessorSave
    where TEntity : class
{
    public ITableAccess Table { get; } = table;
    public TableRows<TEntity> Rows { get; } = table.Rows.As<TEntity>();

    public void Save()
    {
        Table.Save();
    }
}
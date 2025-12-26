namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public static class TableRowsExtensions
{
    extension(ITableRows tableColumns)
    {
        public TableRows<TEntity> As<TEntity>()
        where TEntity : class
        {
            return new(tableColumns);
        }
    }
}
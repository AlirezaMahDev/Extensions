namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public static class TableRowsExtensions
{
    public static TableRows<TEntity> As<TEntity>(this ITableRows tableColumns)
        where TEntity : class
    {
        return new(tableColumns);
    }
}
namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public static class TableRowExtensions
{
    public static TableRow<TEntity> As<TEntity>(this ITableRow tableRow)
        where TEntity : class
    {
        return new(tableRow);
    }
}
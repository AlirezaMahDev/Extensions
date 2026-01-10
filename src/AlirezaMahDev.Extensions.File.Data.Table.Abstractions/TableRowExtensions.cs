namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public static class TableRowExtensions
{
    extension(ITableRow tableRow)
    {
        public TableRow<TEntity> As<TEntity>()
            where TEntity : class
        {
            return new(tableRow);
        }
    }
}
namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public static class TableAccessExtensions
{
    extension(ITableAccess dataLocation)
    {
        public TableAccess<TEntity> As<TEntity>()
            where TEntity : class
        {
            return new(dataLocation);
        }
    }
}
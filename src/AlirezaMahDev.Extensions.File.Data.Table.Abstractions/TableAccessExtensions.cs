namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public static class TableAccessExtensions
{
    public static TableAccess<TEntity> As<TEntity>(this ITableAccess dataLocation)
        where TEntity : class
    {
        return new(dataLocation);
    }
}
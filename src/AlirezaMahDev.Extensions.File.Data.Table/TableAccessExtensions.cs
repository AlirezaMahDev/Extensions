using Microsoft.Extensions.DependencyInjection;

using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table;

public static class TableAccessExtensions
{
    public static ITableAccess AsTable(this IDataLocation dataLocation)
    {
        return dataLocation.DataAccess.FileAccess.Provider.GetRequiredService<TableAccessFactory>()
            .GetOrCreate(dataLocation);
    }
}
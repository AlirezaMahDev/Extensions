using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.File.Data.Table;

public static class TableAccessExtensions
{
    extension(IDataLocation dataLocation)
    {
        public ITableAccess AsTable()
        {
            return dataLocation.DataAccess.FileAccess.Provider.GetRequiredService<TableAccessFactory>()
                .GetOrCreate(dataLocation);
        }
    }
}
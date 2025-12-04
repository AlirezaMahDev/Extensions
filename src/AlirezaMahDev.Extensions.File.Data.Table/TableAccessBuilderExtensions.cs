using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table;

public static class TableAccessBuilderExtensions
{
    extension(IDataAccessBuilder databaseBuilder)
    {
        public ITableAccessBuilder AddTable()
        {
            return new TableAccessBuilder(databaseBuilder);
        }

        public IDataAccessBuilder AddTable(Action<ITableAccessBuilder> action)
        {
            action(AddTable(databaseBuilder));
            return databaseBuilder;
        }
    }
}
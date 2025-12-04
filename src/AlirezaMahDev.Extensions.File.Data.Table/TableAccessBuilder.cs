using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection;
using AlirezaMahDev.Extensions.File.Data.Stack;
using AlirezaMahDev.Extensions.File.Data.Table.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableAccessBuilder : ITableAccessBuilder
{
    public IDataAccessBuilder DataAccessBuilder { get; }

    public TableAccessBuilder(IDataAccessBuilder dataAccessBuilder)
    {
        DataAccessBuilder = dataAccessBuilder;
        dataAccessBuilder.FileBuilder.AddData(builder =>
        {
            builder.AddStack();
            builder.AddCollection();
        });
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<TableAccessFactory>();
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<TableColumnFactory>();
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<TableColumnsFactory>();
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<TableRowsFactory>();
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<TableRowFactory>();
    }
}
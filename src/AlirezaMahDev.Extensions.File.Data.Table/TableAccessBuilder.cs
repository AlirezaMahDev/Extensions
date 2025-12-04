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
        dataAccessBuilder.FileBuilder.Services.AddParameterInstanceFactory<TableAccessFactory>();
        dataAccessBuilder.FileBuilder.Services.AddParameterInstanceFactory<TableColumnFactory>();
        dataAccessBuilder.FileBuilder.Services.AddParameterInstanceFactory<TableColumnsFactory>();
        dataAccessBuilder.FileBuilder.Services.AddParameterInstanceFactory<TableRowsFactory>();
        dataAccessBuilder.FileBuilder.Services.AddParameterInstanceFactory<TableRowFactory>();
    }
}
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
        dataAccessBuilder.FileBuilder.Services.AddSingletonParameterInstanceFactory<TableAccessFactory>();
        dataAccessBuilder.FileBuilder.Services.AddSingletonParameterInstanceFactory<TableColumnFactory>();
        dataAccessBuilder.FileBuilder.Services.AddSingletonParameterInstanceFactory<TableColumnsFactory>();
        dataAccessBuilder.FileBuilder.Services.AddSingletonParameterInstanceFactory<TableRowsFactory>();
        dataAccessBuilder.FileBuilder.Services.AddSingletonParameterInstanceFactory<TableRowFactory>();
    }
}
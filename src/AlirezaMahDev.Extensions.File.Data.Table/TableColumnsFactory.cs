using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableColumnsFactory(IServiceProvider provider)
    : ParameterInstanceFactory<TableColumns, TableAccess>(provider);
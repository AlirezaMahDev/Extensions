using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableRowsFactory(IServiceProvider provider)
    : ParameterInstanceFactory<TableRows, TableAccess>(provider);
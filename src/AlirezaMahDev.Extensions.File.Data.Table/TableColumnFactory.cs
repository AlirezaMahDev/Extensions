using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableColumnFactory(IServiceProvider provider)
    : ParameterInstanceFactory<TableColumn, TableColumnArgs>(provider);
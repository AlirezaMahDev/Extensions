using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableRowFactory(IServiceProvider provider)
    : ParameterInstanceFactory<TableRow, TableRowArgs>(provider);
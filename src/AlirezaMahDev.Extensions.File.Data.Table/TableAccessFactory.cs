using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Table;

internal class TableAccessFactory(IServiceProvider provider)
    : ParameterInstanceFactory<TableAccess, IDataLocation>(provider);
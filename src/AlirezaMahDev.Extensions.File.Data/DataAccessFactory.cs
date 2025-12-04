using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data;

internal class DataAccessFactory(IServiceProvider provider)
    : ParameterInstanceFactory<DataAccess, IFileAccess>(provider);
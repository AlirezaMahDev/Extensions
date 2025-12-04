using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data;

internal class DataBlockAccessorFactory(IServiceProvider provider)
    : ParameterInstanceFactory<DataBlockAccessor, DataBlockAccessorArgs>(provider);
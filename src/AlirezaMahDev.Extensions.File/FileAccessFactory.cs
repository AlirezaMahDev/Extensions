using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File;

internal class FileAccessFactory(IServiceProvider provider)
    : ParameterInstanceFactory<FileAccess, string>(provider);
using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Stack;

internal class StackAccessFactory(IServiceProvider provider)
    : ParameterInstanceFactory<StackAccess, IDataLocation>(provider);
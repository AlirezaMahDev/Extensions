using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Stack;

internal class StackItemsFactory(IServiceProvider provider)
    : ParameterInstanceFactory<StackItems, StackAccess>(provider);
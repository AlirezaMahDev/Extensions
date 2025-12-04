using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Stack;

internal class StackItemFactory(IServiceProvider provider)
    : ParameterInstanceFactory<StackItem, StackItemArgs>(provider);
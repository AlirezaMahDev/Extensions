using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Stack;

internal class StackAccessBuilder : IStackAccessBuilder
{
    public IDataAccessBuilder DataAccessBuilder { get; }

    public StackAccessBuilder(IDataAccessBuilder dataAccessBuilder)
    {
        DataAccessBuilder = dataAccessBuilder;
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<StackAccessFactory>();
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<StackItemFactory>();
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<StackItemsFactory>();
    }
}
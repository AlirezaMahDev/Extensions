using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Stack;

public static class StackAccessBuilderExtensions
{
    extension(IDataAccessBuilder databaseBuilder)
    {
        public IStackAccessBuilder AddStack()
        {
            return new StackAccessBuilder(databaseBuilder);
        }

        public IDataAccessBuilder AddStack(Action<IStackAccessBuilder> action)
        {
            action(databaseBuilder.AddStack());
            return databaseBuilder;
        }
    }
}
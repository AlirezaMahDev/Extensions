using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.File.Data.Stack;

public static class StackAccessExtensions
{
    extension(IDataLocation dataLocation)
    {
        public IStackAccess AsStack()
        {
            return dataLocation.DataAccess.FileAccess.Provider.GetRequiredService<StackAccessFactory>()
                .GetOrCreate(dataLocation);
        }
    }

    extension(IStackAccess dataLocation)
    {
        public StackAccess<TEntity> As<TEntity>()
        where TEntity : unmanaged
        {
            return new(dataLocation);
        }
    }
}
using Microsoft.Extensions.DependencyInjection.Extensions;

using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data;

internal class DataAccessBuilder : IDataAccessBuilder
{
    public DataAccessBuilder(IFileBuilder fileBuilder)
    {
        FileBuilder = fileBuilder;
        fileBuilder.ExtensionsBuilder.Services.TryAddSingleton<IDataService, DataService>();
        fileBuilder.ExtensionsBuilder.Services.TryAddSingleton<DataAccessFactory>();
    }

    public IFileBuilder FileBuilder { get; }
}
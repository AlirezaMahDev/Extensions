using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Abstractions;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlirezaMahDev.Extensions.File.Data;

internal class DataAccessBuilder : IDataAccessBuilder
{
    public DataAccessBuilder(IFileBuilder fileBuilder)
    {
        FileBuilder = fileBuilder;
        fileBuilder.Services.TryAddSingleton<IDataService, DataService>();
        fileBuilder.Services.TryAddSingleton<DataAccessFactory>();
    }

    public IFileBuilder FileBuilder { get; }
}
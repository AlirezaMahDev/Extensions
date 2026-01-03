using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.File.Data;

public static class DataAccessExtensions
{
    extension(IFileAccess fileAccess)
    {
        public IDataAccess AsData()
        {
            return fileAccess.Provider
                .GetRequiredService<DataAccessFactory>()
                .GetOrCreate(fileAccess);
        }
    }

    extension(IFileBuilder builder)
    {
        public IDataAccessBuilder AddData()
        {
            return new DataAccessBuilder(builder);
        }

        public IFileBuilder AddData(Action<IDataAccessBuilder> action)
        {
            action(builder.AddData());
            return builder;
        }
    }
}
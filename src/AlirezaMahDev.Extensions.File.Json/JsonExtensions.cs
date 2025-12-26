using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.File.Json.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.File.Json;

public static class JsonExtensions
{
    extension(IFileBuilder builder)
    {
        public IJsonFileBuilder AddJson()
        {
            return new JsonBuilder(builder);
        }

        public IFileBuilder AddJson(Action<IJsonFileBuilder> action)
        {
            action(AddJson(builder));
            return builder;
        }
    }

    extension(IFileAccess fileAccess)
    {
        public IJsonAccess<TEntity> AsJson<TEntity>()
            where TEntity : class
        {
            return fileAccess.Provider
                .GetRequiredService<JsonAccessFactory<TEntity>>()
                .GetOrCreate(fileAccess);
        }
    }
}
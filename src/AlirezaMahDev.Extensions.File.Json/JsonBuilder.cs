using AlirezaMahDev.Extensions.File.Json.Abstractions;
using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Json;

internal class JsonBuilder : IJsonFileBuilder
{
    public IFileBuilder FileBuilder { get; }

    public JsonBuilder(IFileBuilder fileBuilder)
    {
        FileBuilder = fileBuilder;
        FileBuilder.Services.AddParameterInstanceFactory(typeof(JsonAccessFactory<>));
    }
}
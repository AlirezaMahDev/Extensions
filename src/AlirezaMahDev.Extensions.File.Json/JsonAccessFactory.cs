using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Json;

internal class JsonAccessFactory<TEntity>(IServiceProvider provider)
    : ParameterInstanceFactory<JsonAccess<TEntity>, IFileAccess>(provider)
    where TEntity : class;
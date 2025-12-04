using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

internal class CollectionAccessFactory(IServiceProvider provider)
    : ParameterInstanceFactory<CollectionAccess, IDataLocation>(provider);
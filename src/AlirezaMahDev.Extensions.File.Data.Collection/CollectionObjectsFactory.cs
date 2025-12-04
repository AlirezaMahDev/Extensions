using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

internal class CollectionObjectsFactory(IServiceProvider provider)
    : ParameterInstanceFactory<CollectionObjects, CollectionAccess>(provider);
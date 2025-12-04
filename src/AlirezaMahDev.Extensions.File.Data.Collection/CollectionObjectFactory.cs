using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

internal class CollectionObjectFactory(IServiceProvider provider)
    : ParameterInstanceFactory<CollectionObject, CollectionObjectArgs>(provider);
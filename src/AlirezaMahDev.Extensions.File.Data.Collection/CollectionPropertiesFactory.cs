using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

internal class CollectionPropertiesFactory(IServiceProvider provider)
    : ParameterInstanceFactory<CollectionProperties, ICollectionObjectBase>(provider);
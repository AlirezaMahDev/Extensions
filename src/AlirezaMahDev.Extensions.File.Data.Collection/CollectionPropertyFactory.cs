using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

internal class CollectionPropertyFactory(IServiceProvider provider)
    : ParameterInstanceFactory<CollectionProperty, CollectionPropertyArgs>(provider);
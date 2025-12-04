using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;
using AlirezaMahDev.Extensions.ParameterInstance;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

internal class CollectionAccessBuilder : ICollectionAccessBuilder
{
    public IDataAccessBuilder DataAccessBuilder { get; }

    public CollectionAccessBuilder(IDataAccessBuilder dataAccessBuilder)
    {
        DataAccessBuilder = dataAccessBuilder;
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<CollectionAccessFactory>();
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<CollectionObjectFactory>();
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<CollectionObjectsFactory>();
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<CollectionPropertiesFactory>();
        dataAccessBuilder.FileBuilder.ExtensionsBuilder.Services.AddParameterInstanceFactory<CollectionPropertyFactory>();
    }
}
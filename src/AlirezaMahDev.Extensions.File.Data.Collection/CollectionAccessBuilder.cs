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
        dataAccessBuilder.FileBuilder.Services.AddParameterInstanceFactory<CollectionAccessFactory>();
        dataAccessBuilder.FileBuilder.Services.AddParameterInstanceFactory<CollectionObjectFactory>();
        dataAccessBuilder.FileBuilder.Services.AddParameterInstanceFactory<CollectionObjectsFactory>();
        dataAccessBuilder.FileBuilder.Services.AddParameterInstanceFactory<CollectionPropertiesFactory>();
        dataAccessBuilder.FileBuilder.Services.AddParameterInstanceFactory<CollectionPropertyFactory>();
    }
}
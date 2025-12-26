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
        dataAccessBuilder.FileBuilder.Services.AddSingletonParameterInstanceFactory<CollectionAccessFactory>();
        dataAccessBuilder.FileBuilder.Services.AddSingletonParameterInstanceFactory<CollectionObjectFactory>();
        dataAccessBuilder.FileBuilder.Services.AddSingletonParameterInstanceFactory<CollectionObjectsFactory>();
        dataAccessBuilder.FileBuilder.Services.AddSingletonParameterInstanceFactory<CollectionPropertiesFactory>();
        dataAccessBuilder.FileBuilder.Services.AddSingletonParameterInstanceFactory<CollectionPropertyFactory>();
    }
}
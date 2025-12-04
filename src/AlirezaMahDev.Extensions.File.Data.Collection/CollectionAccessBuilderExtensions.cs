using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

public static class CollectionAccessBuilderExtensions
{
    extension(IDataAccessBuilder databaseBuilder)
    {
        public ICollectionAccessBuilder AddCollection()
        {
            return new CollectionAccessBuilder(databaseBuilder);
        }

        public IDataAccessBuilder AddCollection(Action<ICollectionAccessBuilder> action)
        {
            action(AddCollection(databaseBuilder));
            return databaseBuilder;
        }
    }
}
using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

public interface ICollectionAccessBuilder
{
    IDataAccessBuilder DataAccessBuilder { get; }
}
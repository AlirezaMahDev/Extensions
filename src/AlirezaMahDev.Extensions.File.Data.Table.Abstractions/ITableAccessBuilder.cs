using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Table.Abstractions;

public interface ITableAccessBuilder
{
    IDataAccessBuilder DataAccessBuilder { get; }
}
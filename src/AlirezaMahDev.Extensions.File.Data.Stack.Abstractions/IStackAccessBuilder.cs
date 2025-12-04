using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

public interface IStackAccessBuilder
{
    IDataAccessBuilder DataAccessBuilder { get; }
}
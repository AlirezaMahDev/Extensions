using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

public interface IStackItems : IDataLocationItem, IDataCollection<IStackItem>
{
    IStackAccess StackAccess { get; }
}
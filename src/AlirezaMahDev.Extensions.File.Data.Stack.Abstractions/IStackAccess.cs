using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

public interface IStackAccess : IDataLocationItem
{
    IStackItems Items { get; }
    int Size { get; set; }
}
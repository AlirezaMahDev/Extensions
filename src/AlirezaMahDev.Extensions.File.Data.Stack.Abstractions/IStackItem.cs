using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

public interface IStackItem : IDataCollectionItem<StackItemValue>
{
    DataBlockMemory Data { get; }
}
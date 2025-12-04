using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

public readonly struct StackItem<TValue>(IStackItem item)
    where TValue : unmanaged
{
    public IStackItem Item { get; } = item;
    public ref TValue RefValue => ref Item.Data.AsRefValue<TValue>().RefValue;
}
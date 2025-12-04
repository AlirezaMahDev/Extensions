namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

public static class StackItemExtensins
{
    public static StackItem<TEntity> As<TEntity>(this IStackItem stackItem)
        where TEntity : unmanaged
    {
        return new(stackItem);
    }
}
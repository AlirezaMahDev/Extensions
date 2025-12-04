namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

public static class StackItemsExtensins
{
    public static StackItems<TEntity> As<TEntity>(this IStackItems stackItems)
        where TEntity : unmanaged
    {
        return new(stackItems);
    }
}
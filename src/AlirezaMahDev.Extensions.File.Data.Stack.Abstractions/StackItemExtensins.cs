namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

public static class StackItemExtensins
{
    extension(IStackItem stackItem)
    {
        public StackItem<TEntity> As<TEntity>()
            where TEntity : unmanaged
        {
            return new(stackItem);
        }
    }
}
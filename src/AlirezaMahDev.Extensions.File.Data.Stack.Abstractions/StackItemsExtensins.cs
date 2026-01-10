namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

public static class StackItemsExtensins
{
    extension(IStackItems stackItems)
    {
        public StackItems<TEntity> As<TEntity>()
            where TEntity : unmanaged
        {
            return new(stackItems);
        }
    }
}
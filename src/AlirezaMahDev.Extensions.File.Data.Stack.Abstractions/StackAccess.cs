using System.Runtime.CompilerServices;

namespace AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

public readonly struct StackAccess<TEntity>
    where TEntity : unmanaged
{
    public IStackAccess Stack { get; }

    public StackAccess(IStackAccess stack)
    {
        Stack = stack;
        Items = stack.Items.As<TEntity>();
        if (Items.Count == 0)
        {
            Stack.Size = Unsafe.SizeOf<TEntity>();
        }
    }

    public StackItems<TEntity> Items { get; }
}
using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Stack.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Stack;

internal class StackAccess : IStackAccess
{
    public StackAccess(
        IDataLocation dataLocation,
        StackItemsFactory stackItemsFactory)
    {
        LocationValue = dataLocation.GetOrAdd<StackAccessValue>(".stacks");
        Items = stackItemsFactory.GetOrCreate(this);
    }

    public IStackItems Items { get; }
    public DataLocation<StackAccessValue> LocationValue { get; }
    public IDataLocation Location => LocationValue.Location;

    public ref String64 RefKey => ref LocationValue.RefKey;
    public ref StackAccessValue RefValue => ref LocationValue.RefValue;

    public int Size
    {
        get => RefValue.Size;
        set
        {
            if (Items.Count != 0)
            {
                throw new NotSupportedException("Cannot change size of items when items count != 0");
            }

            RefValue.Size = value;
        }
    }

    public void Save()
    {
        LocationValue.Save();
        Items.Save();
    }
}
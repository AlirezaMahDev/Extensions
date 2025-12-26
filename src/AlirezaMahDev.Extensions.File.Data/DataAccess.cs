using System.Runtime.CompilerServices;

using AlirezaMahDev.Extensions.File.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.File.Data;

internal class DataAccess : IDataAccess
{
    internal DataLocationFactory LocationFactory { get; }

    public DataBlock Block { get; }
    public IFileAccess FileAccess { get; }
    public IDataLocation Root { get; }


    public DataAccess(IFileAccess fileAccess, IServiceProvider provider)
    {
        FileAccess = fileAccess;
        Block = ActivatorUtilities.CreateInstance<DataBlock>(provider, this);
        LocationFactory = ActivatorUtilities.CreateInstance<DataLocationFactory>(provider, this);

        if (BlockRefValue.RefValue.Last == 0L)
        {
            BlockRefValue.RefValue.Last =
                Unsafe.SizeOf<DataFileAccessValue>() + Unsafe.SizeOf<DataLocationValue>();
        }
        Root = LocationFactory.GetOrCreate(Unsafe.SizeOf<DataFileAccessValue>());
    }

    public DataBlockMemoryRefValue<DataFileAccessValue> BlockRefValue =>
        Block.ReadRefValue<DataFileAccessValue>(0L);

    public long GenerateId(int length)
    {
        return Interlocked.Add(ref BlockRefValue.RefValue.Last, length) - length;
    }
}
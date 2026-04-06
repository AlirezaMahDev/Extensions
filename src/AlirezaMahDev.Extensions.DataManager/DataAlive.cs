using System.Runtime.ConstrainedExecution;

using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

internal sealed class DataAlive : CriticalFinalizerObject, IDataAlive
{
    private readonly DataMapFilePart _dataMapFilePart;

    public DataAlive(DataMapFilePart dataMapFilePart)
    {
        _dataMapFilePart = dataMapFilePart;
        Interlocked.Increment(ref _dataMapFilePart.AliveCount);
    }

    ~DataAlive()
    {
        Interlocked.Decrement(ref _dataMapFilePart.AliveCount);
    }
}
namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate ValueTask DataLocationAsyncAction<TValue>(DataLocation<TValue> location,
    CancellationToken cancellationToken = default)
    where TValue : unmanaged, IDataValue<TValue>;
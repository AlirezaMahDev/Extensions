namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate ValueTask UpdateDataLocationAsyncAction<TValue>(DataLocation<TValue> location,
    CancellationToken cancellationToken = default)
    where TValue : unmanaged, IDataValue<TValue>;
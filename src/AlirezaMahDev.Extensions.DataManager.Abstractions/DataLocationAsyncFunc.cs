namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public delegate ValueTask<TResult> DataLocationAsyncFunc<TValue, TResult>(DataLocation<TValue> location,
    CancellationToken cancellationToken = default)
    where TValue : unmanaged, IDataValue<TValue>;